namespace Orders;
public class OrderRestaurants
{
    public string id { get; set; }
    public Order[] Orders { get; set; }
}
public class Order
{
    public string id { get; set; }
    public string restaurantId { get; set; }
    public string GuestEmail { get; set; }
    public bool Takeaway { get; set; }
    public string Date { get; set; }
    public string Hour { get; set; }
    public string Min { get; set; }
    public string Duration { get; set; }
    public Table[] Tables { get; set; }
    public OrderedDish[] Orders { get; set; }
}
public class OrderedDish
{
    public string dishId { get; set; }
    public string Name { get; set; }
    public int Servings { get; set; }
    public int Price { get; set; }
}
public class Table
{
    public int Row { get; set; }
    public int Col { get; set; }
}

