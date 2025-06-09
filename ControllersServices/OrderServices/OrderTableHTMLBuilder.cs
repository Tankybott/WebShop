using Models.DatabaseRelatedModels;
using Services.OrderServices.Interfaces;
using System.Text;
using System.Threading.Tasks;


namespace Services.OrderServices
{
    public class OrderTableHTMLBuilder : IOrderTableHTMLBuilder
    {
        public string BuildHtml(OrderHeader order)
        {
            var sb = new StringBuilder();

            sb.Append($@"
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            margin: 20px;
                        }}
                        h2 {{
                            margin-bottom: 10px;
                        }}
                        table {{
                            width: 100%;
                            border-collapse: collapse;
                            margin-top: 20px;
                        }}
                        th, td {{
                            border: 1px solid #ccc;
                            padding: 8px;
                            text-align: left;
                        }}
                        th {{
                            background-color: #f2f2f2;
                        }}
                    </style>
                </head>
                <body>
                    <h2>Order Summary</h2>
                    <p><strong>Order ID:</strong> {order.Id}</p>

                    <table>
                        <thead>
                            <tr>
                                <th>Product ID</th>
                                <th>Product Name</th>
                                <th>Price</th>
                                <th>Quantity</th>
                            </tr>
                        </thead>
                        <tbody>");

            foreach (var item in order.OrderDetails)
            {
                sb.Append($@"
                    <tr>
                        <td>{item.ProductId}</td>
                        <td>{item.ProductName}</td>
                        <td>{item.Price:C}</td>
                        <td>{item.Quantity}</td>
                    </tr>");
            }

            sb.Append(@"
                        </tbody>
                    </table>
                </body>
                </html>
            ");

            return sb.ToString();
        }
    }
}
