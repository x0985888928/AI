# 專案重點報告： NBC實作
## 開發架構
   - 主框架： Python
## 資料檔
   - `diabetes.csv`  
   - `hepatitis.data.csv`  
   - `glass.csv`  
   - **注意：** 資料中有遺失值 (missing values) 的地方，已以 `0` 進行填補。


## 程式流程： 
1. **讀取資料 (Read CSV)**  
   - 透過 `pandas.read_csv()` 載入 CSV 檔  
   - 取得資料的 row 與 column 數量（假設最後一欄為標籤 / 類別）

2. **離散化判斷 (Discretization)**  
   - 若某欄位的「不同值 (unique values) 數量」> 18，則視為「連續屬性」  
     - 預設以 10 等分 (bins=10) 的方式，將最大值與最小值之間等寬切分  
     - 將每個 bin 中的資料替換為該 bin 的平均值  
     - 例如：把一欄的資料切成 10 組後，每組各自計算一個平均數，再把該組內的所有原始值替換為這個平均數  
   - 若某欄位 unique 值數量 ≤ 18，則視為「離散屬性」，不做分箱。

3. **NBC（Naive Bayes Classifier）訓練流程**  
   - 假設第 `n` 欄為「類別 (class)」  
   - 找出所有類別並計算各類別的筆數 → 先驗機率 (prior)  
   - 蒐集「屬性值在某類別下出現的次數」→ Likelihood（若要避免 0 機率，則使用 Laplace smoothing）  
   - 結合 Prior 與 Likelihood，得到 Posterior

4. **Dirichlet 先驗**  
   - 假設每個類別的「初始先驗」都為 1  
   - 再將這些先驗加總後求平均值，作為後續（或可自行設計不同 α）  
   - 實際計算時，與一般先驗邏輯類似，只是在乘機率時，乘上「Dirichlet 先驗」。

5. **預測與 Accuracy**  
   - 對測試資料（或同一資料集）中的每筆樣本，各自計算在每個類別下的後驗機率  
   - 取概率最大者作為預測結果  
   - 與真實類別比對得到正確率 (accuracy)

6. **Laplace smoothing**  
   - 後驗機率 (Posterior) 中，每個屬性值於不同類別下的計數 `count + 1`  
   - 分母 `類別總筆數 + 該屬性的離散後類別總數`  
   - 避免某些值在某類別下計數為 0，造成最終機率歸零。

7. **K-fold cross validation (未完成)**  
   - 程式碼內部有示範性註解：  
     - 例如把資料切成 5 份（K=5）  
     - 迴圈中，第 `k` 份當測試，其餘當訓練  
     - 每次計算 Accuracy 後，再將 5 次的結果平均即可。  


## 可擴展方向
   - 完成 K-fold cross validation


