# LaundryPro: Laundry Service Management System

**Student:** Isimbi Mushimire Iris

**Student ID:** 27121

**Programme:** Software Engineering, Adventist University of Central Africa (AUCA)


## 1. Project description
LaundryPro helps a small laundry shop register customer orders, calculate prices automatically, track each order from reception to delivery, record payments, and see a daily summary. It replaces the paper notebook that many shops use.

The full Software Requirements Specification (functional requirements) is in [`1_SRS/LaundryPro_SRS.pdf`](1_SRS/LaundryPro_SRS.pdf).

## 2. Environment
- Operating system: Windows
- IDE: Visual Studio
- Language and platform: C# on .NET 10 (console application)

## 3. Project structure
```
LaundryPro-Project/
├── 1_SRS/
│   └── LaundryPro_SRS.pdf
├── 2_Code/
│   └── LaundryPro/
│       ├── Program.cs
│       └── LaundryPro.csproj
├── screenshots/
├── .gitignore
└── README.md
```

## 4. Features of the prototype
| Option | Function |
|--------|----------|
| 1 | New order (name, 10-digit phone, service, weight) with automatic price and receipt |
| 2 | Show all orders with balance and status |
| 3 | Move an order to the next stage: Received, Washing, Ready, Delivered |
| 4 | Record a full or partial payment |
| 5 | Search orders by customer name or phone number |
| 6 | Cancel an order (only while it is still "Received") |
| 7 | Daily summary: orders per status, money collected, money still owed |
| 0 | Exit |

Business rules: stages cannot be skipped, and an order cannot be delivered until it is fully paid. All input is validated.

Prices per kg: Wash only 500 RWF, Wash and iron 800 RWF, Dry cleaning 2000 RWF, Iron only 300 RWF.

## 5. How to run
1. Open the `2_Code/LaundryPro` folder in Visual Studio (File > Open > Folder).
2. Open the Developer PowerShell (View > Terminal).
3. Run:
   ```
   cd 2_Code/LaundryPro
   dotnet restore
   dotnet run
   ```
4. Type the number of an option and press Enter.

## 6. Screenshots

### 6.1 Main menu
![Main menu](screenshots/mainmenu.png)

### 6.2 New order and receipt
![New order](screenshots/neworder.png)

### 6.3 All orders
![All orders](screenshots/showallorders.png)

### 6.4 Cancel order
![Cancel order](screenshots/cancelorder.png)

### 6.5 Daily summary
![Summary](screenshots/dailysummary.png)

## 7. Conclusion
The prototype uses an `Order` class, a `List<Order>`, arrays for services and stages, methods for each menu option, and `TryParse` validation so that wrong input does not crash the program. Data is kept in memory only; a future version can save orders in a database or file.
