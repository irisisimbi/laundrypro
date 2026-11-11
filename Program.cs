using System;
using System.Collections.Generic;

namespace LaundryPro
{
    // One laundry order
    class Order
    {
        public int Id;
        public string Customer;
        public string Phone;
        public string Service;
        public double Kg;
        public double Price;
        public double Paid;
        public string Status;

        public double Balance
        {
            get { return Price - Paid; }
        }
    }

    class Program
    {
        static List<Order> orders = new List<Order>();
        static int nextId = 1;

        // Service names and price per kg (in RWF)
        static string[] services = { "Wash only", "Wash and iron", "Dry cleaning", "Iron only" };
        static double[] prices = { 500, 800, 2000, 300 };

        // The order moves through these stages in this order
        static string[] stages = { "Received", "Washing", "Ready", "Delivered" };

        static void Main(string[] args)
        {
            bool running = true;

            while (running)
            {
                ShowMenu();
                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1": NewOrder(); break;
                    case "2": ShowOrders(); break;
                    case "3": MoveToNextStage(); break;
                    case "4": RecordPayment(); break;
                    case "5": SearchOrders(); break;
                    case "6": CancelOrder(); break;
                    case "7": ShowSummary(); break;
                    case "0":
                        running = false;
                        Console.WriteLine("Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a number from the menu.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine();
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("=================================");
            Console.WriteLine("   LAUNDRYPRO - LAUNDRY SYSTEM");
            Console.WriteLine("=================================");
            Console.WriteLine("1. New order");
            Console.WriteLine("2. Show all orders");
            Console.WriteLine("3. Move an order to the next stage");
            Console.WriteLine("4. Record a payment");
            Console.WriteLine("5. Search orders (name or phone)");
            Console.WriteLine("6. Cancel an order");
            Console.WriteLine("7. Daily summary");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");
        }

        static void NewOrder()
        {
            Console.Write("Customer name: ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Name cannot be empty.");
                return;
            }

            Console.Write("Phone number (digits only, 10 digits): ");
            string phone = (Console.ReadLine() ?? "").Trim();
            if (phone.Length != 10 || !AllDigits(phone))
            {
                Console.WriteLine("Invalid phone number. It must have exactly 10 digits.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Services:");
            for (int i = 0; i < services.Length; i++)
            {
                Console.WriteLine("  {0}. {1} ({2} RWF per kg)", i + 1, services[i], prices[i]);
            }
            Console.Write("Choose a service (1-" + services.Length + "): ");
            int service;
            if (!int.TryParse(Console.ReadLine(), out service) || service < 1 || service > services.Length)
            {
                Console.WriteLine("Invalid service.");
                return;
            }

            Console.Write("Weight in kg: ");
            double kg;
            if (!double.TryParse(Console.ReadLine(), out kg) || kg <= 0 || kg > 100)
            {
                Console.WriteLine("Invalid weight. It must be a number between 0 and 100.");
                return;
            }

            Order o = new Order();
            o.Id = nextId;
            nextId++;
            o.Customer = name.Trim();
            o.Phone = phone;
            o.Service = services[service - 1];
            o.Kg = kg;
            o.Price = kg * prices[service - 1];
            o.Paid = 0;
            o.Status = stages[0];
            orders.Add(o);

            Console.WriteLine();
            Console.WriteLine("----------- RECEIPT -----------");
            Console.WriteLine("Order number : " + o.Id);
            Console.WriteLine("Customer     : " + o.Customer);
            Console.WriteLine("Service      : " + o.Service);
            Console.WriteLine("Weight       : " + o.Kg + " kg");
            Console.WriteLine("Total price  : " + o.Price + " RWF");
            Console.WriteLine("Status       : " + o.Status);
            Console.WriteLine("-------------------------------");
        }

        static void ShowOrders()
        {
            if (orders.Count == 0)
            {
                Console.WriteLine("No orders yet. Add one first (option 1).");
                return;
            }

            PrintHeader();
            foreach (Order o in orders)
            {
                PrintOrder(o);
            }
        }

        static void MoveToNextStage()
        {
            Order o = AskForOrder();
            if (o == null) return;

            int index = Array.IndexOf(stages, o.Status);

            if (o.Status == "Cancelled")
            {
                Console.WriteLine("This order was cancelled.");
                return;
            }
            if (index == stages.Length - 1)
            {
                Console.WriteLine("This order is already delivered.");
                return;
            }

            string next = stages[index + 1];

            // Business rule: an order cannot be delivered until it is fully paid
            if (next == "Delivered" && o.Balance > 0)
            {
                Console.WriteLine("Cannot deliver: the customer still owes " + o.Balance + " RWF. Record the payment first (option 4).");
                return;
            }

            o.Status = next;
            Console.WriteLine("Order " + o.Id + " is now: " + o.Status);
        }

        static void RecordPayment()
        {
            Order o = AskForOrder();
            if (o == null) return;

            if (o.Status == "Cancelled")
            {
                Console.WriteLine("This order was cancelled.");
                return;
            }
            if (o.Balance <= 0)
            {
                Console.WriteLine("This order is already fully paid.");
                return;
            }

            Console.WriteLine("Amount still to pay: " + o.Balance + " RWF");
            Console.Write("Amount received (RWF): ");
            double amount;
            if (!double.TryParse(Console.ReadLine(), out amount) || amount <= 0 || amount > o.Balance)
            {
                Console.WriteLine("Invalid amount. It must be more than 0 and not more than the balance.");
                return;
            }

            o.Paid += amount;
            Console.WriteLine("Payment recorded. Remaining balance: " + o.Balance + " RWF");
        }

        static void SearchOrders()
        {
            Console.Write("Enter customer name or phone number: ");
            string text = (Console.ReadLine() ?? "").Trim().ToLower();
            bool found = false;

            foreach (Order o in orders)
            {
                if (text.Length > 0 && (o.Customer.ToLower().Contains(text) || o.Phone.Contains(text)))
                {
                    if (!found) PrintHeader();
                    PrintOrder(o);
                    found = true;
                }
            }

            if (!found)
            {
                Console.WriteLine("No matching order found.");
            }
        }

        static void CancelOrder()
        {
            Order o = AskForOrder();
            if (o == null) return;

            // Only orders that have not started can be cancelled
            if (o.Status != "Received")
            {
                Console.WriteLine("Only orders with status 'Received' can be cancelled.");
                return;
            }

            o.Status = "Cancelled";
            Console.WriteLine("Order " + o.Id + " has been cancelled.");
        }

        static void ShowSummary()
        {
            if (orders.Count == 0)
            {
                Console.WriteLine("No orders yet. Add one first (option 1).");
                return;
            }

            double collected = 0;
            double unpaid = 0;
            foreach (Order o in orders)
            {
                if (o.Status == "Cancelled") continue;
                collected += o.Paid;
                unpaid += o.Balance;
            }

            Console.WriteLine("Total orders: " + orders.Count);
            foreach (string stage in stages)
            {
                Console.WriteLine("  " + stage + ": " + CountStatus(stage));
            }
            Console.WriteLine("  Cancelled: " + CountStatus("Cancelled"));
            Console.WriteLine("Money collected : " + collected + " RWF");
            Console.WriteLine("Money still owed: " + unpaid + " RWF");
        }

        // ---------- helper methods ----------

        static Order AskForOrder()
        {
            if (orders.Count == 0)
            {
                Console.WriteLine("No orders yet. Add one first (option 1).");
                return null;
            }

            ShowOrders();
            Console.WriteLine();
            Console.Write("Enter the order number: ");
            int id;
            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Invalid number.");
                return null;
            }

            foreach (Order o in orders)
            {
                if (o.Id == id) return o;
            }

            Console.WriteLine("Order not found.");
            return null;
        }

        static int CountStatus(string status)
        {
            int count = 0;
            foreach (Order o in orders)
            {
                if (o.Status == status) count++;
            }
            return count;
        }

        static bool AllDigits(string text)
        {
            foreach (char c in text)
            {
                if (c < '0' || c > '9') return false;
            }
            return true;
        }

        static void PrintHeader()
        {
            Console.WriteLine("{0,-4} {1,-14} {2,-12} {3,-14} {4,5} {5,8} {6,8}  {7}",
                "No.", "Customer", "Phone", "Service", "Kg", "Price", "Balance", "Status");
            Console.WriteLine(new string('-', 88));
        }

        static void PrintOrder(Order o)
        {
            Console.WriteLine("{0,-4} {1,-14} {2,-12} {3,-14} {4,5} {5,8} {6,8}  {7}",
                o.Id, o.Customer, o.Phone, o.Service, o.Kg, o.Price, o.Balance, o.Status);
        }
    }
}
