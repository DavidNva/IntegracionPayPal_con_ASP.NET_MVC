using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;//Estas tres primeras son para la API Rest
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using paypalTest.Models.PaypalOrder;
using paypalTest.Models.PaypalTransaction;

namespace paypalTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> About()//Será la vista para pago efectuado
        {
            //id de autorizacion para obtener el dinero
            string token = Request.QueryString["token"];/*Obtenemos el atributo token de la url*/
            
            bool status = false;
            string respuesta = string.Empty;

            using (var client = new HttpClient())//Una peticion para el servicio de rest
            {
                var userName = "COLOCAR USER NAME CON EL QUE SE REGISTRO EN PAYPAL";
                var password = "COLOCAR PASSWORD CON EL QUE SE REGISTRO EN PAYPAL";

                client.BaseAddress = new Uri("https://api-m.sandbox.paypal.com");

                var authToken = Encoding.ASCII.GetBytes($"{userName}:{password}");//Convertimos los textos en tipo bytes y se envian en el cliente
                //como autorizacion en tipo basico
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
                //El body de api captura es vacio, pero se deben colocar las llaves vacias
                var data = new StringContent("{}", Encoding.UTF8, "application/json");//del tipo application json

                HttpResponseMessage response = await client.PostAsync($"/v2/checkout/orders/{token}/capture", data);//Esta ejecutando un metodo post del tipo asyncrono
                //el await es para indicar un stop, pero no avanza hasta que termina este metodo
                //Estamos pasando toda la data convertida
                status = response.IsSuccessStatusCode;
                ViewData["Status"] = status;
                if (status)
                {
                    var jsonRespuesta = response.Content.ReadAsStringAsync().Result;//Toda la respuesta generada
                    PaypalTransaction objeto = JsonConvert.DeserializeObject<PaypalTransaction>(jsonRespuesta);//Convertimos                                                                              //el json respuesta en un elemento paypalTransaction
                    ViewData["IdTransaccion"] = objeto.purchase_units[0].payments.captures[0].id;//El id de transacction
                }
            }

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        //las tareas asincronas significa que un ejemplo son metodos que pueden continuar sin necesidad del tiempo que les llegue a tomar
        //asincrono no cesesariamente espera a que termina un metodo para continuar con el otro
        [HttpPost]
        public async Task<JsonResult> Paypal(string precio, string producto)
        {
            bool status = false;
            string respuesta = string.Empty;

            using (var client = new HttpClient())//Una peticion para el servicio de rest
            {
                var userName = "USAR EL USER NAME CON EL QUE SE REGISTRO EN PAYPAL";
                var password = "USE EL PASSWORD CON EL QUE SE REGISTRO EN PAYPAL";

                client.BaseAddress = new Uri("https://api-m.sandbox.paypal.com");

                var authToken = Encoding.ASCII.GetBytes($"{userName}:{password}");//Convertimos los textos en tipo bytes y se envian en el cliente
                //como autorizacion en tipo basico
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));


                var orden = new PaypalOrder()//El body
                {
                    intent = "CAPTURE",
                    purchase_units = new List<Models.PaypalOrder.PurchaseUnit>() {//Se hace referencia al la clase el models en PaypalOrder
                         new Models.PaypalOrder.PurchaseUnit()
                         {
                             amount= new Models.PaypalOrder.Amount() {
                                 currency_code = "USD",
                                 value = precio
                             }, 
                             description= producto
                        }
                     },
                    application_context = new ApplicationContext()
                    {
                        brand_name = "Mi Tienda CodeTest",
                        landing_page = "NO_PREFERENCE",
                        user_action = "PAY_NOW",//Accion para que paypal muestre el monto de pago
                        return_url = "https://localhost:44312/Home/About",//cuando se aprueba la solicitud de cobro
                        cancel_url = "https://localhost:44312/Home/Index" //cuando cancela la operacion
                    }
                };
                var json = JsonConvert.SerializeObject(orden);//convertimos a json
                var data = new StringContent(json, Encoding.UTF8, "application/json");//del tipo application json

                HttpResponseMessage response = await client.PostAsync("/v2/checkout/orders", data);//Esta ejecutando un metodo post del tipo asyncrono
                //el await es para indicar un stop, pero no avanza hasta que termina este metodo
                //Estamos pasando toda la data convertida
                status = response.IsSuccessStatusCode;
                if (status)
                {
                    respuesta = response.Content.ReadAsStringAsync().Result;//Guarda todo el resultado
                }
            }

            return Json(new { status = status, respuesta = respuesta }, JsonRequestBehavior.AllowGet);
        }
        //Boton de procesar pago

        //[HttpPost]
        //public async Task<JsonResult> ProcesarPago(List<EN_Carrito> oListaCarrito, EN_Venta oVenta)//Con parametros
        //{//los servicios de paypal obligan a trabajar de manera asincrona
        //    decimal total = 0;
        //    DataTable detalleVenta = new DataTable();
        //    detalleVenta.Locale = new CultureInfo("es-MX");

        //    //Comenzamos a crear las columnas que necesita esta table
        //    detalleVenta.Columns.Add("IdProducto", typeof(string));
        //    detalleVenta.Columns.Add("Cantidad", typeof(int));
        //    detalleVenta.Columns.Add("Total", typeof(decimal));//Esta tabla viene a ser la representacion de la estructura creada en sql (EDetalle_Venta)

        //    foreach (EN_Carrito oCarrito in oListaCarrito)//por cada carrito en la lista carrito
        //    {
        //        decimal subTotal = Convert.ToDecimal(oCarrito.Cantidad.ToString()) * oCarrito.oId_Producto.Precio;

        //        total += subTotal;//Va aumentando el valor de total con cada iteracion
        //        detalleVenta.Rows.Add(new object[]
        //        {
        //            oCarrito.oId_Producto.IdProducto,
        //            oCarrito.Cantidad,
        //            subTotal
        //        });
        //    }
        //    oVenta.MontoTotal = total;
        //    oVenta.Id_Cliente = ((EN_Cliente)Session["Cliente"]).IdCliente;
        //    TempData["Venta"] = oVenta;  //Almacena informacion que vamos a poder compartir a traves de metodos (Todo el obj de venta)
        //    TempData["DetalleVenta"] = detalleVenta; //Almacena todo el dataTable
        //    return Json(new { Status = true, Link = "/Tienda/PagoEfectuado?IdTransaccion=code0001&status=true" }, JsonRequestBehavior.AllowGet);
        //    //Enviamos dos parametros el id de transaccon y un status como true
        //    //Por el momento la estructura es estatica (por lo pronto hasta aqui es una simulacion de paypal)
        //}
    }
}