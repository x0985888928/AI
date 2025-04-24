# 專案重點報告：.NET & AI 數據分析整合

## 開發架構

- 完整流程：資料蒐集 → 建模 → 前後端串接 → 可視化

## 技術選擇

- 主框架：.NET Core
- AI 實現：ML.NET
- 資料處理：人工處理 + EF Core
- 前端：Blazor/MVC

## 專案文件

- 開發歷程：https://hackmd.io/@Homegiz/ByOePFWZ2

## 目前進度

- CORE 架構與部署知識（MVC、Azure、IIS）
- 簡易 CORE 頁面與 CRUD 功能實作
- 資料集獲取與清理完成
- 訓練模型完成
- 更新CI/CD部署流程

## 實施時程

- 第1週：CORE基礎知識與簡單頁面實作
- 第2週：資料集獲取與需求定義  
- 第3-4週：模型訓練與 API 建立 <--- 現在在這
- 第5-6週：前端可視化實作與部署
- 超前進度時研究CI/CD

## 資料處理

1. 資料獲取：Kaggle 數據集（價格預測）
2. 資料前處理：
- 處理方式：
  * 首先將kaggle資料瀏覽後，確認將Model Name切割成品牌、版本、類型(Pro、Plus等等)、ROM
  * 補上異常或缺失值(例如類型為空白者則設定為Normal、ROM空白設定為128GB)
  * 將部分數值資料數值化(將ROM、RAM等欄位移除GB、mAh等多於字串)
  * 將類別欄位編碼 (One-Hot Encoding)
  * 選擇部分特徵訓練與參數優化測試 
- 資料庫導入(目前未執行)

## 模型訓練流程

1. 資料分割：訓練集（75%）與測試集（25%）
2. 模型建構：
- 特徵處理方式如上一章節所述
- 演算法選擇測試三種(FastTree(Decision Tree)、SDCA(線性)、LightGbm(非線性))，使用結果最優的LightGbmRegressor

3. 模型評估：

- 效能指標使用RMSE、R-squared、MAE

4. 模型匯出：產生 model.zip 檔案

## 系統整合

1. 後端實作：

- ASP.NET Core 中載入模型
- 建立預測 API
- 定義輸入/輸出類別

2. 前端實作：

- 使用者輸入表單設計
- Chart.js 視覺化展示
- 預測結果與歷史數據比較

## 部署方案

1. IIS、Azure 或 Docker 容器化
2. 完整 GitHub README 文件
3. 演示影片或圖表化報告

## 可擴展方向

- 自動重訓模型機制
- 多目標預測（價格、銷量）
- 綜合分析儀表板
