# 訓練模型
## 資料處理流程
1. 資料獲取：Kaggle 數據集（價格預測）
2. 資料前處理：
- 處理方式：
  * 首先將kaggle資料瀏覽後，確認將Model Name切割成品牌、版本、類型(Pro、Plus等等)、ROM
  * 補上異常或缺失值(例如類型為空白者則設定為Normal、ROM空白設定為128GB)
  * 將部分數值資料數值化(將ROM、RAM等欄位移除GB、mAh等多於字串)
  * 將類別欄位編碼 (One-Hot Encoding)
  * 選擇部分特徵訓練與參數優化測試 

## 模型訓練流程

1. 資料分割：訓練集（75%）與測試集（25%）
2. 模型建構：
- 特徵處理方式如上一章節所述
- 演算法選擇測試三種(FastTree(Decision Tree)、SDCA(線性)、LightGbm(非線性))，使用結果最優的FastTree

## 模型評估：

- 效能指標使用RMSE、R-squared、MAE

## 模型匯出：使用CORE的Microsoft.ML 打包模型產生 model.zip 檔案
