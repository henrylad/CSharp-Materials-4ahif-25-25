using System.Reflection;
using System.Runtime.CompilerServices;
using haggling_interfaces;

HagglingMarket market = new HagglingMarket();

Console.WriteLine("Loading vendor and customer implementations...");
market.LoadImplementationsFromDirectory();

Console.WriteLine("Starting haggling market simulation...");
for (int i = 0; i < 10; i++)
{
    Console.WriteLine($"--- Simulation Round {i + 1} ---");
    market.RunMarketSimulation();
    Console.WriteLine();
    Thread.Sleep(1000);
}

Console.WriteLine("Market simulation completed. Press any key to exit...");
Console.ReadKey();

public class HagglingMarket
{
    private readonly List<IVendor> _vendors = [];
    private readonly List<ICustomer> _customers = [];
    private IDisplay? _display = null;
    private Type _displayType = null!;
    private readonly Random _random = new();
    private readonly string DLL_DIRECTORY = Path.Combine(Directory.GetCurrentDirectory(), "implementations");

    private class SimpleOffer : IOffer
    {
        public OfferStatus Status { get; set; }
        public IProduct Product { get; set; } = null!;
        public decimal Price { get; set; }
        public PersonType OfferedBy { get; set; }
    }

    private class SimpleVendor : IVendor
    {
        public string Name { get; init; } = null!;
        public int Age { get; init; }
        public IProduct[] Products { get; init; } = [];
        public Percentage Patience { get; set; }
        public IOffer GetStartingOffer(IProduct product, ICustomer customer) => null!;
        public IOffer RespondToOffer(IOffer offer, ICustomer customer) => null!;
        public void AcceptTrade(IOffer offer) { }
        public void StopTrade() { }
    }

    private class SimpleCustomer : ICustomer
    {
        public string Name { get; init; } = null!;
        public int Age { get; init; }
        public Percentage Patience { get; set; }
        public IProduct ChooseProduct(IVendor vendor) => null!;
        public IOffer RespondToOffer(IOffer offer, IVendor vendor) => null!;
        public void AcceptTrade(IOffer offer) { }
        public void StopTrade() { }
    }

    private static IOffer CopyOffer(IOffer offer)
    {
        if (offer == null) return null!;

        return new SimpleOffer
        {
            Status = offer.Status,
            Product = offer.Product,
            Price = offer.Status == OfferStatus.Stopped ? 0 : offer.Price,
            OfferedBy = offer.OfferedBy
        };
    }

    private static IVendor CopyVendor(IVendor vendor)
    {
        if (vendor == null) return null!;

        return new SimpleVendor
        {
            Name = vendor.Name,
            Age = vendor.Age,
            Products = vendor.Products,
            Patience = vendor.Patience
        };
    }

    private static ICustomer CopyCustomer(ICustomer customer)
    {
        if (customer == null) return null!;

        return new SimpleCustomer
        {
            Name = customer.Name,
            Age = customer.Age,
            Patience = customer.Patience
        };
    }

    private void InstatiateDisplay(Type type)
    {
        _display = (IDisplay)Activator.CreateInstance(type)!;
    }

    public void LoadImplementationsFromDirectory()
    {
        if (!Directory.Exists(DLL_DIRECTORY))
        {
            Console.WriteLine($"Please place the DLL files in the '{DLL_DIRECTORY}' directory and restart the program.");
            return;
        }

        var dllFiles = Directory.GetFiles(DLL_DIRECTORY, "*.dll");
        Console.WriteLine($"Found {dllFiles.Length} DLL files to process...");

        foreach (var dllFile in dllFiles)
        {
            LoadImplementationsFromDll(dllFile);
        }

        Console.WriteLine($"Loaded {_vendors.Count} vendor(s), {_customers.Count} customer(s), {(_display != null ? 1 : 0)} display(s)");
    }

    private void LoadImplementationsFromDll(string dllPath)
    {
        Console.WriteLine($"Loading implementations from: {Path.GetFileName(dllPath)}");

        var assembly = Assembly.LoadFrom(dllPath);

        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsAbstract && !type.IsInterface)
            {
                if (typeof(IVendorFactory).IsAssignableFrom(type))
                {
                    for (var i = 0; i < 20; i++)
                    {
                        var mi = type.GetMethod("CreateVendor", BindingFlags.Public | BindingFlags.Static)!;
                        var vendor = (IVendor)mi.Invoke(null, [$"Vendor_{_random.Next(1000, 9999)}", _random.Next(5, 40)])!;
                        _vendors.Add(vendor);
                        Console.WriteLine($"  - Loaded vendor: {vendor.Name} (Age: {vendor.Age})");
                    }
                }
                else if (typeof(ICustomerFactory).IsAssignableFrom(type))
                {
                    for (var i = 0; i < 20; i++)
                    {
                        var mi = type.GetMethod("CreateCustomer", BindingFlags.Public | BindingFlags.Static)!;
                        var customer = (ICustomer)mi.Invoke(null, [$"Customer_{_random.Next(1000, 9999)}", _random.Next(5, 40)])!;
                        _customers.Add(customer);
                        Console.WriteLine($"  - Loaded customer: {customer.Name} (Age: {customer.Age})");
                    }
                }
                else if (typeof(IDisplay).IsAssignableFrom(type))
                {
                    InstatiateDisplay(_displayType = type);
                }
            }
        }
    }

    /*Start by generating random vendor and customer to simulate trades
     * Vendor schickt Offers -> Customer wählt Produkt aus ->
     * Vendor macht offer -> Customer akzeptiert oder macht counteroffer bis Einigung erzielt oder abgebrochen wird
    */
    public void RunMarketSimulation()
    {
        try
        {
            //setting up Vendor
            IVendor vendor = _vendors[_random.Next(_vendors.Count)];
            ICustomer customer = _customers[_random.Next(_customers.Count)];
            IOffer currentOffer = null!;
            bool tradeCompleted = false;

            InstatiateDisplay(_displayType);
            _display?.ShowProducts(vendor.Products, vendor, customer);
            Thread.Sleep(1000);

            IProduct product = customer.ChooseProduct(vendor);

            currentOffer = vendor.GetStartingOffer(product, customer);
            _display?.ShowOffer(CopyOffer(currentOffer), CopyVendor(vendor), CopyCustomer(customer));

            while (currentOffer.Status != OfferStatus.Stopped && !tradeCompleted)
            {
                Thread.Sleep(1000);
                CallSilenced(() => currentOffer = customer.RespondToOffer(CopyOffer(currentOffer), vendor));
                _display?.ShowOffer(CopyOffer(currentOffer), CopyVendor(vendor), CopyCustomer(customer));

                if (currentOffer.Status == OfferStatus.Accepted)
                {
                    vendor.AcceptTrade(CopyOffer(currentOffer));
                    tradeCompleted = true;
                }
                else if (currentOffer.Status == OfferStatus.Stopped)
                {
                    vendor.StopTrade();
                    tradeCompleted = true;
                }

                if (!tradeCompleted)
                {
                    currentOffer = vendor.RespondToOffer(CopyOffer(currentOffer), customer);
                    Thread.Sleep(1000);
                    _display?.ShowOffer(CopyOffer(currentOffer), CopyVendor(vendor), CopyCustomer(customer));

                    if (currentOffer.Status == OfferStatus.Accepted)
                    {
                        CallSilenced(() => customer.AcceptTrade(CopyOffer(currentOffer)));
                        tradeCompleted = true;
                    }
                    else if (currentOffer.Status == OfferStatus.Stopped)
                    {
                        CallSilenced(() => customer.StopTrade());
                        tradeCompleted = true;
                    }
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void CallSilenced(Action act)
    {
        var original = Console.Out;
        try
        {
            Console.SetOut(TextWriter.Null); // Schreibbefehle verschwinden
            act();
        }
        finally
        {
            Console.SetOut(original);
        }
    }
}


