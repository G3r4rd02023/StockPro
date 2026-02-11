# StockPro

Reports
GET /api/report/inventory — Reporte de inventario (JSON).
GET /api/report/movements?startDate=X&endDate=Y — Reporte de movimientos.
GET /api/report/inventory/csv — Descarga CSV de inventario.
GET /api/report/movements/csv — Descarga CSV de movimientos.

QRCode
GET /api/qrcode/product/{id} — Descarga QR como PNG.
GET /api/qrcode/product/{id}/base64 — QR en base64 para display.
GET /api/qrcode/scan?sku=X&productId=Y — Búsqueda por escaneo.

Alerts
GET /api/alert — Listar alertas (filtro no leídas).
GET /api/alert/unread-count — Conteo para badge en sidebar.
PATCH /api/alert/{id}/read — Marcar como leída.
PATCH /api/alert/read-all — Marcar todas como leídas.
POST /api/alert/check — Detección manual.

Dashboard
GET /api/dashboard

StockMovement
POST /api/stockmovements - Registrar entradas y salidas
GET /api/stockmovements - Listar todos los movimientos
GET /api/stockmovements?productId=9190b75c-a9e3-4ca2-b663-9cf31b3142d2 - Filtrar por producto especifico
GET /api/stockmovements?type=0 - Filtrar por tipo
GET /api/stockmovements?userId=9190b75c-a9e3-4ca2-b663-9cf31b3142d2 - Filtrar por usuario
GET /api/stockmovements/{movementId} - Obtener movimiento por ID 


Products

GET {{baseUrl}}/api/products - Listar todos los productos

GET {{baseUrl}}/api/products?search=Galaxy - Buscar por nombre o SKU

GET {{baseUrl}}/api/products?categoryId=ID_DE_CATEGORIA - Filtrar por categoria

GET {{baseUrl}}/api/products?lowStock=true  Filtrar por bajo stock

GET {{baseUrl}}/api/products/{{productId}} Obtener producto por ID

POST {{baseUrl}}/api/products Crear producto

DELETE {{baseUrl}}/api/products/{{productId}} Eliminar producto


Categories

POST https://localhost:7043/api/categories Crear una categoria

GET {{baseUrl}}/api/categories Obtener todas las categorias

GET {{baseUrl}}/api/categories/{{categoryId}} Obtener por Id

PUT {{baseUrl}}/api/categories/{{categoryId}} Editar categoria

DELETE {{baseUrl}}/api/categories/{{categoryId}} Eliminar categoria


Auth
POST {{baseUrl}}/api/auth/register registrar nuevo usuario

POST {{baseUrl}}/api/auth/login  iniciar sesion

POST {{baseUrl}}/api/auth/logout  cerrar sesion









