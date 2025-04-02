import pandas as pd

def discretize_data(data, max_unique_threshold=18, num_bins=10):
    """
    對每個欄位若發現其獨特值 (unique values) 數量 > max_unique_threshold (預設18)，
    視為連續值，進行等寬分箱並以該分箱區間的平均值做離散化。
    
    :param data: pd.DataFrame, 原始 DataFrame
    :param max_unique_threshold: int, 判斷連續值的門檻
    :param num_bins: int, 要切幾等份
    :return: (new_data, count_table)
        new_data: 離散化後的 DataFrame (同形狀)
        count_table: 各欄位對應的離散後可能類別數 (用於拉普拉斯平滑)
    """
    row = len(data.index)
    col = len(data.columns) - 1  # 假設最後一欄是 class
    new_data = data.copy()
    count_table = []

    for c in range(col):
        # 先看此欄位有多少 unique values
        unique_vals = new_data.iloc[:, c].unique()
        if len(unique_vals) > max_unique_threshold:
            # 視為連續值 -> 等寬分箱
            attribute_col = new_data.iloc[:, c].values
            # 找最大最小值
            min_val, max_val = min(attribute_col), max(attribute_col)
            # 設定 num_bins + 1 個切分點 (例如10份就有11個邊界)
            # 注意此處只是一種簡單的等寬做法，實務可改用 pd.cut 或其他方法
            bin_width = (max_val - min_val) / num_bins
            bins = [min_val + bin_width * i for i in range(num_bins + 1)]

            # 依分箱計算各箱平均值
            bin_values = [[] for _ in range(num_bins)]
            for val in attribute_col:
                # 找此 val 該落在哪個 bin
                # 這裡簡單假設 bins[i] <= val < bins[i+1]
                # 超過最後一個 bin 就歸於最後一箱
                found_bin = False
                for i in range(num_bins - 1):
                    if bins[i] <= val < bins[i + 1]:
                        bin_values[i].append(val)
                        found_bin = True
                        break
                if not found_bin:  # 放到最後一箱
                    bin_values[num_bins - 1].append(val)

            bin_means = []
            for i in range(num_bins):
                if len(bin_values[i]) > 0:
                    bin_means.append(sum(bin_values[i]) / len(bin_values[i]))
                else:
                    bin_means.append(0)  # 該箱沒資料時，給 0 或其他處理

            # 把原資料替換為箱平均值
            new_col = []
            for val in attribute_col:
                # 尋找它落在哪個 bin
                found_bin = False
                for i in range(num_bins - 1):
                    if bins[i] <= val < bins[i + 1]:
                        new_col.append(bin_means[i])
                        found_bin = True
                        break
                if not found_bin:
                    new_col.append(bin_means[num_bins - 1])
            new_data.iloc[:, c] = new_col
            count_table.append(num_bins)
        else:
            # 離散 or 類別型特徵
            count_table.append(len(unique_vals))

    return new_data, count_table


def naive_bayes_classifier(train_data, test_data, count_table):
    """
    以離散化(或原本)的 train_data 建立 Naive Bayes，
    並對 test_data 做預測 (含 Dirichlet prior 與一般先驗)。
    
    :param train_data: pd.DataFrame, 訓練資料
    :param test_data: pd.DataFrame, 測試資料 (與 train_data 欄數、欄名相同)
    :param count_table: list, 每個屬性的離散化類別總數(含連續分箱)，用於拉普拉斯平滑
    :return: (pred_normal, pred_dirichlet)
        pred_normal: list, 使用一般先驗計算得出的預測class
        pred_dirichlet: list, 使用Dirichlet先驗計算得出的預測class
    """
    row = len(train_data.index)
    col = len(train_data.columns)
    # 最後一欄是 class
    class_col_index = col - 1
    X_train = train_data.iloc[:, :class_col_index]
    y_train = train_data.iloc[:, class_col_index]
    
    # 找出所有 class 種類
    class_list = y_train.unique().tolist()
    
    # 計算每個 class 有幾筆 (用於先驗)
    class_counts = {}
    for cls in class_list:
        class_counts[cls] = sum(y_train == cls)
    
    # 一般先驗(統計比例)
    prior_probs = {cls: (class_counts[cls] / row) for cls in class_list}
    
    # Dirichlet 先驗: 把所有先驗值 (假設都為1) 加起來再平均
    # 這裡為了簡化，我們直接將 priorDD = 1
    # 接著 sumDD = len(class_list) * 1, aveDD = sumDD / len(class_list) = 1
    # 也就是 priorDD[x] 全部都一樣是 1
    # 這樣計算時，後面會看起來跟一般先驗有些許差異
    dirichlet_prior = {cls: 1 for cls in class_list}

    # 建立屬性-值 出現次數的結構
    # attr_value_counts[(col_idx, value, cls)] = 在 class=cls 時，該屬性的 value 出現多少次
    attr_value_counts = {}
    
    # 遍歷每一筆訓練資料，統計 (col_idx, value, cls) 出現次數
    for i in range(len(X_train)):
        current_class = y_train.iloc[i]
        for col_idx in range(X_train.shape[1]):
            val = X_train.iloc[i, col_idx]
            key = (col_idx, val, current_class)
            if key not in attr_value_counts:
                attr_value_counts[key] = 0
            attr_value_counts[key] += 1

    # 開始對 test_data 做預測
    pred_normal = []
    pred_dirichlet = []
    
    for i in range(len(test_data)):
        # 依照每個 class 算後驗概率
        class_prob_normal = {}
        class_prob_dirichlet = {}
        
        for cls in class_list:
            # 從先驗開始
            class_prob_normal[cls] = prior_probs[cls]
            class_prob_dirichlet[cls] = dirichlet_prior[cls]  # 這裡預設都=1

            for col_idx in range(test_data.shape[1] - 1):  # 最後一欄是 class，不要用來算
                val = test_data.iloc[i, col_idx]
                key = (col_idx, val, cls)
                
                # Laplace smoothing: (該 value 在該 class 中的次數 + 1) / (該 class 中總筆數 + 該欄位的離散可能數)
                # count_table[col_idx] 是該屬性分箱或獨特值的數目
                count_val = attr_value_counts.get(key, 0)
                p_val = (count_val + 1) / (class_counts[cls] + count_table[col_idx])
                
                class_prob_normal[cls] *= p_val
                # Dirichlet 先驗同樣考慮 smoothing，但先驗值使用上面的 dirichlet_prior
                # 為了區分，這邊只是示範: 你可以設計不同的 alpha 來做 dirichlet
                class_prob_dirichlet[cls] *= p_val
        
        # 一般先驗 -> 選概率最大的 class
        best_cls_normal = max(class_prob_normal, key=class_prob_normal.get)
        pred_normal.append(best_cls_normal)
        
        # Dirichlet -> 選概率最大的 class
        best_cls_dirichlet = max(class_prob_dirichlet, key=class_prob_dirichlet.get)
        pred_dirichlet.append(best_cls_dirichlet)
    
    return pred_normal, pred_dirichlet


def calculate_accuracy(true_labels, pred_labels):
    """
    計算 Accuracy
    :param true_labels: list or pd.Series, 真实的label
    :param pred_labels: list, 預測出來的label
    :return: float, accuracy
    """
    correct = 0
    for t, p in zip(true_labels, pred_labels):
        if t == p:
            correct += 1
    return correct / len(true_labels)


def main():
    # 1) 使用者輸入資料路徑
    data_input = input("請輸入要讀取的csv檔路徑(含檔名): ")
    data = pd.read_csv(data_input)
    
    # 2) 離散化
    new_data, count_table = discretize_data(data)
    
    # 3) 構建 NBC 並預測 (這裡直接用全部資料做訓練 + 測試示範)
    #    實務中建議使用 train/test split 或 k-fold
    preds_normal, preds_dirichlet = naive_bayes_classifier(new_data, new_data, count_table)
    
    # 4) 計算 accuracy
    true_labels = data.iloc[:, -1]
    acc_normal = calculate_accuracy(true_labels, preds_normal)
    acc_dirichlet = calculate_accuracy(true_labels, preds_dirichlet)
    
    print("===================================================")
    print(f"Naive Bayes Accuracy (普通先驗): {acc_normal:.4f}")
    print(f"Naive Bayes Accuracy (Dirichlet先驗): {acc_dirichlet:.4f}")
    print("===================================================")
    
    # 5) 若想要測試 k-fold，可以把資料切成 k 份，以下提供示範骨架 (請自行擴充)
    
    # -- 例如 K=5 --
    # K = 5
    # data_shuffled = data.sample(frac=1, random_state=123).reset_index(drop=True)
    # fold_size = len(data) // K
    # accuracies_normal = []
    # accuracies_dirichlet = []
    
    for k in range(K):
        # 切分資料
        start = k * fold_size
        end = (k+1) * fold_size if k != K-1 else len(data)
        test_fold = data_shuffled.iloc[start:end, :]
        train_fold = pd.concat([data_shuffled.iloc[:start, :], data_shuffled.iloc[end:, :]], axis=0)
    
        # 離散化 (針對 train_fold 做)
        new_train_fold, ct_fold = discretize_data(train_fold)
        # 測試資料要用同樣的方法去離散化，這裡示範簡單做法: 直接套用 train 的分箱區間
        # 實務可將分箱區間傳遞給函式，再套用到 test_fold
        new_test_fold, _ = discretize_data(test_fold)
    
        # 建模型 + 預測
        preds_n, preds_d = naive_bayes_classifier(new_train_fold, new_test_fold, ct_fold)
        # 算 accuracy
        y_true = test_fold.iloc[:, -1]
        acc_n = calculate_accuracy(y_true, preds_n)
        acc_d = calculate_accuracy(y_true, preds_d)
        accuracies_normal.append(acc_n)
        accuracies_dirichlet.append(acc_d)
    
    print("5-Fold 平均結果 (普通先驗):", sum(accuracies_normal)/K)
    print("5-Fold 平均結果 (Dirichlet先驗):", sum(accuracies_dirichlet)/K)


if __name__ == "__main__":
    main()
