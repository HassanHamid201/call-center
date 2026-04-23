#!/bin/bash
set -e

BASE_URL="http://localhost:3000"
API_URL="http://localhost:5000/api"
TOKEN=""
PASS=0
FAIL=0

check() {
  local code="$1"
  local expected="$2"
  local name="$3"
  if [ "$code" = "$expected" ]; then
    echo "  ✅ $name ($code)"
    PASS=$((PASS+1))
  else
    echo "  ❌ $name (got $code, expected $expected)"
    FAIL=$((FAIL+1))
  fi
}

echo "════════════════════════════════════════════════════════════"
echo "  TADAWI PORTAL — COMPREHENSIVE TEST SUITE"
echo "════════════════════════════════════════════════════════════"
echo ""

# ── 1. SERVICE HEALTH ───────────────────────────────────────
echo "1. SERVICE HEALTH"
echo "   Frontend: $(curl -s -o /dev/null -w "%{http_code}" $BASE_URL/)"
echo "   Backend:  $(curl -s -o /dev/null -w "%{http_code}" $API_URL/)"
echo ""

# ── 2. PUBLIC FRONTEND ROUTES ───────────────────────────────
echo "2. PUBLIC FRONTEND ROUTES"
for route in "/" "/login" "/branches" "/doctors" "/search" "/faq"; do
  code=$(curl -s -o /dev/null -w "%{http_code}" "$BASE_URL$route")
  check "$code" "200" "FE $route"
done
echo ""

# ── 3. ADMIN FRONTEND ROUTES (no auth → should return 200 SPA page) ──
echo "3. ADMIN FRONTEND ROUTES (SPA shell loads regardless)"
for route in "/admin" "/admin/users" "/admin/branches" "/admin/doctors" "/admin/specialties" "/admin/sectors" "/admin/faq" "/admin/audit"; do
  code=$(curl -s -o /dev/null -w "%{http_code}" "$BASE_URL$route")
  check "$code" "200" "FE $route"
done
echo ""

# ── 4. AUTHENTICATION ───────────────────────────────────────
echo "4. AUTHENTICATION"
# 4a. Login with seeded admin
echo "   4a. Admin login..."
LOGIN_RESP=$(curl -s -X POST "$API_URL/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@tadawi.med","password":"Admin123!"}')
TOKEN=$(echo "$LOGIN_RESP" | python3 -c "import sys,json; print(json.load(sys.stdin).get('token',''))" 2>/dev/null || echo "")
if [ -n "$TOKEN" ] && [ "$TOKEN" != "null" ]; then
  echo "      ✅ Login successful, token received"
  PASS=$((PASS+1))
else
  echo "      ❌ Login failed: $LOGIN_RESP"
  FAIL=$((FAIL+1))
fi

# 4b. Invalid login
echo "   4b. Invalid login..."
code=$(curl -s -o /dev/null -w "%{http_code}" -X POST "$API_URL/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"bad@user.com","password":"wrong"}')
check "$code" "401" "Invalid login returns 401"

echo ""

# ── 5. PUBLIC API ENDPOINTS (no auth) ───────────────────────
echo "5. PUBLIC API ENDPOINTS (no auth)"
# All endpoints are [Authorize] protected (correct for internal call center portal)
for endpoint in "branches" "doctors" "specialties" "sectors" "faq" "search?query=test"; do
  code=$(curl -s -o /dev/null -w "%{http_code}" "$API_URL/$endpoint")
  check "$code" "401" "GET /api/$endpoint (unauthorized)"
done
echo ""

# ── 6. PROTECTED API ENDPOINTS (with auth) ──────────────────
echo "6. PROTECTED API ENDPOINTS (with auth)"
if [ -n "$TOKEN" ]; then
  AUTH="Authorization: Bearer $TOKEN"

  # 6a. Admin stats
  code=$(curl -s -o /dev/null -w "%{http_code}" -H "$AUTH" "$API_URL/admin/stats")
  check "$code" "200" "GET /api/admin/stats"

  # 6b. Admin users list
  code=$(curl -s -o /dev/null -w "%{http_code}" -H "$AUTH" "$API_URL/admin/users")
  check "$code" "200" "GET /api/admin/users"

  # 6c. Doctors CRUD
  echo "   6c. Doctors CRUD..."
  DOCTOR_JSON='{"displayName":"Test Doctor","firstName":"Test","lastName":"Doctor","email":"test.doc@tadawi.com","phone":"0500000001","mobile":"0500000001","branchId":"91385c98-ebd3-5973-9d8c-116f9e486800","specialtyId":"cd76245f-512e-5f8e-8025-ee81d1619b8b","sectorId":"ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0","classification":"أخصائي","nationality":"Saudi","consultationFee":200,"isActive":true}'

  # Create (should work if admin)
  CREATE_RESP=$(curl -s -X POST "$API_URL/doctors" -H "$AUTH" -H "Content-Type: application/json" -d "$DOCTOR_JSON")
  DOC_ID=$(echo "$CREATE_RESP" | python3 -c "import sys,json; d=json.load(sys.stdin); print(d.get('id','') or d.get('data',{}).get('id',''))" 2>/dev/null || echo "")
  if [ -n "$DOC_ID" ] && [ "$DOC_ID" != "null" ] && [ "$DOC_ID" != "" ]; then
    echo "      ✅ POST /api/doctors (created id=$DOC_ID)"
    PASS=$((PASS+1))

    # Update
    UPD_JSON=$(echo "$DOCTOR_JSON" | python3 -c "import sys,json; d=json.load(sys.stdin); d['displayName']='Updated Doctor'; print(json.dumps(d))")
    code=$(curl -s -o /dev/null -w "%{http_code}" -X PUT "$API_URL/doctors/$DOC_ID" -H "$AUTH" -H "Content-Type: application/json" -d "$UPD_JSON")
    check "$code" "200" "PUT /api/doctors/{id}"

    # Delete
    code=$(curl -s -o /dev/null -w "%{http_code}" -X DELETE "$API_URL/doctors/$DOC_ID" -H "$AUTH")
    check "$code" "204" "DELETE /api/doctors/{id}"
  else
    echo "      ❌ POST /api/doctors failed: $CREATE_RESP"
    FAIL=$((FAIL+1))
  fi

  # 6d. FAQ CRUD
  echo "   6d. FAQ CRUD..."
  FAQ_JSON='{"question":"Test question?","answer":"Test answer.","category":"General","order":99,"isActive":true}'
  CREATE_RESP=$(curl -s -X POST "$API_URL/faq" -H "$AUTH" -H "Content-Type: application/json" -d "$FAQ_JSON")
  FAQ_ID=$(echo "$CREATE_RESP" | python3 -c "import sys,json; d=json.load(sys.stdin); print(d.get('id','') or d.get('data',{}).get('id',''))" 2>/dev/null || echo "")
  if [ -n "$FAQ_ID" ] && [ "$FAQ_ID" != "null" ] && [ "$FAQ_ID" != "" ]; then
    echo "      ✅ POST /api/faq (created id=$FAQ_ID)"
    PASS=$((PASS+1))

    code=$(curl -s -o /dev/null -w "%{http_code}" -X PUT "$API_URL/faq/$FAQ_ID" -H "$AUTH" -H "Content-Type: application/json" -d '{"question":"Updated?","answer":"Updated.","category":"General","order":99,"isActive":true}')
    check "$code" "200" "PUT /api/faq/{id}"

    code=$(curl -s -o /dev/null -w "%{http_code}" -X DELETE "$API_URL/faq/$FAQ_ID" -H "$AUTH")
    check "$code" "204" "DELETE /api/faq/{id}"
  else
    echo "      ❌ POST /api/faq failed: $CREATE_RESP"
    FAIL=$((FAIL+1))
  fi

  # 6e. Search
  echo "   6e. Search..."
  SEARCH_RESP=$(curl -s -H "$AUTH" "$API_URL/search?query=doctor")
  TOTAL=$(echo "$SEARCH_RESP" | python3 -c "import sys,json; print(json.load(sys.stdin).get('totalCount',-1))" 2>/dev/null || echo "-1")
  if [ "$TOTAL" -ge 0 ] 2>/dev/null; then
    echo "      ✅ Search returned $TOTAL results"
    PASS=$((PASS+1))
  else
    echo "      ❌ Search failed"
    FAIL=$((FAIL+1))
  fi

  # 6f. Lookup endpoints
  echo "   6f. Lookup endpoints..."
  for endpoint in "doctors/classifications" "doctors/availability-statuses" "faq/categories"; do
    code=$(curl -s -o /dev/null -w "%{http_code}" -H "$AUTH" "$API_URL/$endpoint")
    check "$code" "200" "GET /api/$endpoint"
  done

else
  echo "   Skipping authenticated tests (no token)"
fi

echo ""
echo "════════════════════════════════════════════════════════════"
echo "  RESULTS: $PASS passed, $FAIL failed"
echo "════════════════════════════════════════════════════════════"
if [ $FAIL -gt 0 ]; then
  exit 1
fi
