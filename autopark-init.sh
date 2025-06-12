#!/bin/bash

set -e
sudo apt install jq

# Получаем IP адрес Windows-хоста из WSL
HOST_IP=$(ip route | grep default | awk '{print $3}')
BASE_URL="http://${HOST_IP}:8080"
COOKIE_FILE="cookies.txt"

echo "== 1. Установка пароля администратора ==";
curl -s -X POST "$BASE_URL/api/Auth/setup-admin" \
  -H "Content-Type: application/json" \
  -d '{"password": "String123@"}'

echo "== 2. Вход под администратором и сохранение cookie =="
curl -s -X POST "$BASE_URL/login?useCookies=true" \
  -H "Content-Type: application/json" \
  -c "$COOKIE_FILE" \
  -d '{"email": "admin@example.com", "password": "String123@"}'

echo "== 3. Создание двух предприятий =="
curl -s -X POST "$BASE_URL/api/Enterprises" \
  -H "Content-Type: application/json" -b "$COOKIE_FILE" \
  -d '{"name": "Предприятие 1", "address": "Адрес 1"}'

curl -s -X POST "$BASE_URL/api/Enterprises" \
  -H "Content-Type: application/json" -b "$COOKIE_FILE" \
  -d '{"name": "Предприятие 2", "address": "Адрес 2"}'

echo "== 4. Присоединение к первому предприятию =="
curl -s -X POST "$BASE_URL/api/Auth/attach-to-enterprise" \
  -H "Content-Type: application/json" -b "$COOKIE_FILE" \
  -d "{\"enterpriseId\": 1}"

echo "== 4. Присоединение ко второму предприятию =="
curl -s -X POST "$BASE_URL/api/Auth/attach-to-enterprise" \
  -H "Content-Type: application/json" -b "$COOKIE_FILE" \
  -d "{\"enterpriseId\": 2}"

echo "== 5. Получение идентификаторов предприятий =="
ENT_IDS=$(curl -s -X GET "$BASE_URL/api/Enterprises" \
  -H "Accept: application/json" -b "$COOKIE_FILE" | jq -r '.[].id')
readarray -t ENT_IDS_ARRAY <<< "$ENT_IDS"

echo "== 6. Создание модели транспорта (BrandModel) =="
curl -s -X POST "$BASE_URL/api/BrandModels" \
  -H "Content-Type: application/json" -b "$COOKIE_FILE" \
  -d '{
    "brandName": "LADA",
    "modelName": "Granta",
    "transportType": 0,
    "fuelType": 1,
    "seatsNumber": 4,
    "maximumLoadCapacityInKillograms": 1000
  }'

echo "== 7. Создание транспортных средств по одному на каждое предприятие =="
VEHICLE_IDS=()
for i in "${!ENT_IDS_ARRAY[@]}"; do
  curl -s -X POST "$BASE_URL/api/Vehicles" \
    -H "Content-Type: application/json" -b "$COOKIE_FILE" \
    -d "{
      \"name\": \"Гранта $i\",
      \"price\": 1000,
      \"mileageInKilometers\": 1000,
      \"color\": \"белый\",
      \"registrationNumber\": \"123ABC\",
      \"brandModelId\": 1,
      \"enterpriseId\": ${ENT_IDS_ARRAY[$i]}
    }"

  VEH_ID=$(curl -s "$BASE_URL/api/Vehicles" -b "$COOKIE_FILE" | jq -r '.[] | select(.name=="Гранта '$i'") | .id')
  VEHICLE_IDS+=($VEH_ID)
done

echo "== 8. Создание водителей по одному на каждое предприятие =="
DRIVER_IDS=()
for i in "${!ENT_IDS_ARRAY[@]}"; do
  curl -s -X POST "$BASE_URL/api/Drivers" \
    -H "Content-Type: application/json" -b "$COOKIE_FILE" \
    -d "{
      \"firstName\": \"Водитель$i\",
      \"lastName\": \"Тестовый\",
      \"dateOfBirth\": \"1990-01-01T00:00:00Z\",
      \"salary\": 100000,
      \"enterpriseId\": ${ENT_IDS_ARRAY[$i]}
    }"

  DRIVER_ID=$(curl -s "$BASE_URL/api/Drivers" -b "$COOKIE_FILE" | jq -r '.[] | select(.firstName=="Водитель'$i'") | .id')
  DRIVER_IDS+=($DRIVER_ID)
done

echo "== 9. Привязка водителей к соответствующим транспортным средствам =="
for i in "${!DRIVER_IDS[@]}"; do
  curl -s -X PUT "$BASE_URL/api/Drivers" \
    -H "Content-Type: application/json" -b "$COOKIE_FILE" \
    -d "{
      \"id\": ${DRIVER_IDS[$i]},
      \"firstName\": \"Водитель$i\",
      \"lastName\": \"Тестовый\",
      \"dateOfBirth\": \"1990-01-01T00:00:00Z\",
      \"salary\": 100000,
      \"enterpriseId\": ${ENT_IDS_ARRAY[$i]},
      \"attachedVehicleId\": ${VEHICLE_IDS[$i]}
    }"
done

echo ""
echo "== 10. Список BrandModels =="
curl -s "$BASE_URL/api/BrandModels" -b "$COOKIE_FILE" | jq

echo ""
echo "== 11. Список предприятий =="
curl -s "$BASE_URL/api/Enterprises" -b "$COOKIE_FILE" | jq

echo ""
echo "== 12. Список водителей =="
curl -s "$BASE_URL/api/Drivers" -b "$COOKIE_FILE" | jq

echo ""
echo "== 13. Список транспортных средств =="
curl -s "$BASE_URL/api/Vehicles" -b "$COOKIE_FILE" | jq

echo ""
echo "== The End? =="
