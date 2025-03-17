# Integración de PayPal en ASP.NET MVC

Este proyecto es un ejemplo de cómo integrar PayPal en una aplicación ASP.NET MVC (versión .NET 4.7.2). La integración permite procesar pagos a través de la API REST de PayPal y puede ser utilizada como base para proyectos de comercio electrónico, plataformas de donaciones o cualquier sistema que requiera pagos en línea.

---

## 🚀 Requisitos

Antes de comenzar, asegúrate de tener lo siguiente:

- Visual Studio 2019 o superior
- .NET Framework 4.7.2
- Cuenta de desarrollador en [PayPal Developer](https://developer.paypal.com/)
- Postman (opcional, para probar llamadas a la API de PayPal)
---

## Arquitectura del Proyecto

![Integracion PayPal_Con_ASP NET_MCV](https://github.com/user-attachments/assets/cc29e72a-3fe2-4eb0-9347-11c31a0acf69)

---

## 🔑 Configuración de Credenciales en PayPal

Para integrar PayPal en tu aplicación, necesitas generar credenciales API:

1. **Registrarse en PayPal Developer**  
   - Ve a [PayPal Developer](https://developer.paypal.com/).
   - Inicia sesión o crea una cuenta.

2. **Crear una aplicación de prueba**  
   - Dirígete a **Dashboard > My Apps & Credentials**.
   - Selecciona **Sandbox** y haz clic en **Create App**.
   - Asigna un nombre a la aplicación y selecciona la cuenta de negocio.
   - Guarda el **Client ID** y **Secret**, ya que los necesitarás para autenticarte.

3. **Configurar Webhooks (Opcional)**  
   - En la misma sección de **My Apps & Credentials**, puedes agregar webhooks para recibir notificaciones de pagos completados.

---

## Instalación del Proyecto

1. **Clona el repositorio**
   ```bash
   git clone https://github.com/tu-repo/paypal-integration-mvc.git
   cd paypal-integration-mvc
   ```

2. **Abrir en Visual Studio IDE**  
   - Abre el archivo `PaypalIntegration.sln` en Visual Studio.

3. **Configurar credenciales de PayPal**  
   - En el archivo `appsettings.json` o `Web.config`, agrega las credenciales obtenidas en PayPal:
     ```json
     {
       "PayPal": {
         "ClientId": "TU_CLIENT_ID",
         "ClientSecret": "TU_CLIENT_SECRET",
         "Mode": "sandbox"  // Cambiar a "live" en producción
       }
     }
     ```

4. **Restaurar paquetes NuGet**  
   - En Visual Studio, abre la Consola de Administrador de Paquetes y ejecuta:
     ```powershell
     Update-Package -Reinstall
     ```

5. **Ejecutar la aplicación**  
   - Presiona `F5` o ejecuta en Visual Studio.
   - Accede a `http://localhost:5000` para ver la aplicación en acción.

---

## 🔄 Flujo de Pago con PayPal

1. **El usuario selecciona un producto y procede al pago.**
2. **El sistema crea una orden en PayPal usando la API.**
3. **Se redirige al usuario a PayPal para completar el pago.**
4. **PayPal procesa la transacción y redirige de vuelta a la aplicación.**
5. **La aplicación valida la respuesta y actualiza el estado del pago.**

---

## 📌 Implementación en Código

### 1️⃣ Modelo de Orden (`PayPalOrderModel.cs`)
```csharp
public class PayPalOrderModel {
    public string Intent { get; set; } = "CAPTURE";
    public Payer Payer { get; set; }
    public List<Transaction> Transactions { get; set; }
}
```

### 2️⃣ Controlador de Pago (`PayPalController.cs`)
```csharp
[HttpPost]
public async Task<IActionResult> CreateOrder() {
    var request = new HttpRequestMessage(HttpMethod.Post, "https://api.sandbox.paypal.com/v2/checkout/orders");
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessToken());
    request.Content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");

    var response = await _httpClient.SendAsync(request);
    var content = await response.Content.ReadAsStringAsync();
    return Ok(JsonConvert.DeserializeObject(content));
}
```

### 3️⃣ Captura del Pago (`CaptureOrder`)
```csharp
public async Task<IActionResult> CaptureOrder(string orderId) {
    var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.sandbox.paypal.com/v2/checkout/orders/{orderId}/capture");
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessToken());
    
    var response = await _httpClient.SendAsync(request);
    return Ok(await response.Content.ReadAsStringAsync());
}
```

---

## 🛠️ Adaptación a Otros Proyectos (E-commerce, Donaciones, etc.)

Esta integración se puede aplicar en otros proyectos con pequeñas modificaciones:

- **E-commerce:** Asociar el pago con una orden en la base de datos.
- **Donaciones:** Permitir montos personalizados y agregar una lista de contribuyentes.
- **Suscripciones:** Integrar con `Billing Agreements` de PayPal para pagos recurrentes.

---

## 🔍 Pruebas y Depuración

1. **Pruebas con cuentas de Sandbox**  
   - En PayPal Developer, usa las cuentas de prueba generadas automáticamente.
   - Se puede simular una transacción sin afectar dinero real.

2. **Revisar Logs**  
   - Agregar logs en cada paso de la transacción para depuración.

---

## 📢 Notas Finales

- Antes de ir a producción, asegúrate de cambiar `sandbox` a `live` en la configuración.
- Implementa seguridad con autenticación y validaciones extra.
- Considera usar Webhooks de PayPal para mejorar la validación de pagos.

---

## 📌 Recursos Adicionales

- [Documentación oficial de PayPal API](https://developer.paypal.com/docs/api/overview/)
---

