# ProjectLifeModule
- The following explanation is provided in both Traditional Chinese and English.

## 1.本準則的核心目的是提供一套心法、指引並舉例數個程序的派工、執行緒模組.
The core purpose of this guideline is to provide a mindset, guidance, and examples of several procedures for task assignment and execution thread modules.

| **Item** | **Module** | **功能** |
|----------|--------------|-------------|
| **1** | **MTimer** | 使用計時器，定時執行，只需要實做觸發的函式 |
| **2** | **MRightNow**| 馬上執行指定工作，僅執行一次，後續不再運行 |
| **3** | **MInfiniteLoop** | 馬上執行指定工作，無盡循環的執行，永不停止 |
| **4** | **MFileWatcher**| 監控檔案，指定資料夾路徑及檔案名稱 |
| **5** | **MClock** | 時鐘，指定時間後執行 |

## 2.原理/心法架構:
Principle/Mindset Framework
- 在天地之間，一切皆有法則，天地運行有陰陽，世間萬物有虛實，程式設計亦是如此。本指引手冊以道為本，將天地運行的哲理融入程式設計，展現一個具備陰陽平衡與虛實相生的系統架構方法。
  
  Between heaven and earth, everything follows certain principles. The universe operates through yin and yang, and all things in the world exist in both form and emptiness—programming is no exception. This guide is rooted in the concept of the Dao, integrating the philosophical principles of the universe into software design, presenting a system architecture approach that embodies the balance of yin and yang and the interplay of form and emptiness.

  ![image](https://github.com/user-attachments/assets/be6aea1c-1d37-4765-ba32-fa23e4b91778)

  ![image](https://github.com/user-attachments/assets/ec7ad71d-ad99-41f1-8d62-34a9db99373d)

## 3.陰陽平衡，虛實相生:
- 虛實之道:虛者，無形之效能，實者，有形之結果。程式的虛實相生體現在運行效率與功能表現上。
  
  The Way of Form and Emptiness: Emptiness represents intangible efficiency, while Form represents tangible results. The interplay of form and emptiness in programming is reflected in both operational efficiency and functional performance.

## 4.程式:
- 程式語言	C# .NET8.0
- 開發工具	Visual Studio 2022(17.12.1)
- 版本控制	GitHub
- 套件管理	NuGet

### 4-1.新需求專案:
只須把模組及觸發的函式(核心功能)，添加進去，便能自動運行
![image](https://github.com/user-attachments/assets/7aba5a85-27e7-4a4f-8de7-afbf0ffe0893)

本專案以本機端為運作提供範本，請編譯建置完成後，即可在本機端運作執行。
![image](https://github.com/user-attachments/assets/7ca353e2-15d9-4b87-83ea-35dfc9bf3a17)

![image](https://github.com/user-attachments/assets/7c808db8-a85b-49cb-b1c3-865b500feede)

![image](https://github.com/user-attachments/assets/52cf6bb5-69ed-4cf5-9125-f549599f90cf)

### 4-2.程式專案清單:
| **Item** | **Project** | **功能** |
|----------|--------------|-------------|
| **1** | **HolyGift** | 定義通用常數 |
| **2** | **ILogger**| 紀錄 Log 物件 |
| **3** | **Judgment** | ResultCode定義 |
| **4** | **OLogger**| 儲存 Log 檔案的物件 |
| **5** | **TCTUtility** | 通用方法 |
| **6** | **ProjectLifeModulePalace** | 建立此專案時用的名稱，Git版控 |
| **7** | **ProjectLifeModuleManagement** | 專案生命模組管理 |
| **8** | **ProjectLifeModuleCase** | 專案的案件核心 |
| **9** | **ProjectLifeModuleConsole** | 專案運作Console |

![image](https://github.com/user-attachments/assets/c5a94dcb-14a7-434a-9fb4-9ce0b774f97b)

## 5.致謝:
各位心懷遠志的開發者，請相信您手中的每一行程式碼，都是構建天地的一部分。在陰陽中尋求平衡，在虛實間鑄造永恆，您的創作將成為無可替代的藝術品，於世間閃耀光芒。

我們深信，憑藉您的努力與智慧，這套程式/心法必將臻於至善，為更多人帶來便利與美好。讓我們攜手共創未來，讓您的才華如天際星辰，照亮無數人的前行之路。

敬祝：
編程順利，成果輝煌！
