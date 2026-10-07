# Parking Slot Management API

ASP.NET Core Web API for basement parking slots. No frontend — use Swagger to test.

## How to run

```
dotnet run
```

Open http://localhost:5104/swagger

SQLite file `parking.db` is created on first run with 6 car slots (A1–A6) and 4 bike slots (B1–B4). Delete the db file if you want to reset.

## Fees

- Car: Rs 20 per hour
- Bike: Rs 10 per hour
- Any part of an hour counts as a full hour

## Endpoints

- GET /api/slots — all slots
- GET /api/slots/available?type=Car — free slots (type is optional)
- GET /api/slots/summary — car/bike counts
- POST /api/slots — add a slot `{ "slotNumber": "A7", "slotType": "Car" }`
- POST /api/slots/{id}/park — `{ "vehicleNumber": "TS09AB1234" }`
- POST /api/slots/{id}/exit — frees the slot and returns fee
- DELETE /api/slots/{id} — delete if not occupied
