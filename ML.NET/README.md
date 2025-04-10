# 專案重點報告：.NET & AI 數據分析整合
## 開發架構
- 完整流程：資料蒐集 → 建模 → 前後端串接 → 可視化

## 技術選擇

主框架：.NET Core
- AI 實現：ML.NET 或 Python 訓練後與 .NET 整合
- 資料處理：EF Core
- 前端：Blazor/MVC

## 專案文件

- 開發歷程：https://hackmd.io/@Homegiz/ByOePFWZ2

## 目前進度

- CORE 架構與部署知識（MVC、Azure、IIS）
- 簡易 CORE 頁面與 CRUD 功能實作

## 實施時程

- 第1週：CORE基礎知識與簡單頁面實作
- 第2週：資料集獲取與需求定義
- 第3-4週：模型訓練與 API 建立
- 第5-5週：前端可視化實作與部署

## 資料處理

1. 資料獲取：Kaggle 數據集（價格預測）
2. 資料前處理：
- 類別欄位編碼（One-Hot Encoding）
- 特徵篩選與優化
- 資料庫導入

## 模型訓練流程

1. 資料分割：訓練集（75%）與測試集（25%）
2. 模型建構：

- 特徵處理（類別編碼、數值正規化）
- 演算法選擇（SdcaRegression、LightGbmRegressor）


3. 模型評估：

- 效能指標（RMSE、R-squared、MAE）
- 特徵工程優化（Log轉換、組合特徵）

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