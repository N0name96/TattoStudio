#!/usr/bin/env bash
# test-endpoints.sh — Prueba todos los endpoints de TattoStudio.Api
# Requiere que la API esté corriendo en http://localhost:5018
# Uso: bash test-endpoints.sh

set -uo pipefail

BASE="http://localhost:5018"
PASS=0
FAIL=0
RESP_FILE=$(mktemp)
trap 'rm -f "$RESP_FILE" /tmp/tattostudio_test_img.png' EXIT

# ─── helpers ────────────────────────────────────────────────────────────────

GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Hace la llamada HTTP. Escribe el body en $RESP_FILE y devuelve el HTTP status.
# Uso: STATUS=$(req GET /url [--token T] [--json BODY] [--form K=V ...])
req() {
    local method="$1"; shift
    local url="$1";    shift
    local args=("-s" "-o" "$RESP_FILE" "-w" "%{http_code}" "-X" "$method")
    local token="" body="" form=()

    while [[ $# -gt 0 ]]; do
        case "$1" in
            --token) token="$2"; shift 2 ;;
            --json)  body="$2";  shift 2 ;;
            --form)  form+=("$2"); shift 2 ;;
            *) shift ;;
        esac
    done

    [[ -n "$token" ]] && args+=("-H" "Authorization: Bearer $token")
    if [[ -n "$body" ]]; then
        args+=("-H" "Content-Type: application/json" "-d" "$body")
    fi
    for f in "${form[@]}"; do
        args+=("-F" "$f")
    done

    curl "${args[@]}" "$url"
}

# Extrae el valor de un campo JSON del último response.
extract() {
    grep -o "\"$1\":\"[^\"]*\"" "$RESP_FILE" | head -1 | cut -d'"' -f4
}

assert() {
    local label="$1" expected="$2" actual="$3"
    if [[ "$actual" == "$expected" ]]; then
        echo -e "${GREEN}[PASS]${NC} $label  (HTTP $actual)"
        PASS=$((PASS + 1))
    else
        echo -e "${RED}[FAIL]${NC} $label  — esperado $expected, obtenido $actual"
        cat "$RESP_FILE"
        echo ""
        FAIL=$((FAIL + 1))
    fi
}

# ─── 1. AUTH ────────────────────────────────────────────────────────────────

echo -e "\n${YELLOW}══════ AUTH ══════${NC}"

STATUS=$(req POST "$BASE/api/auth/register" \
    --json '{"email":"testrun@tattostudio.com","password":"Test1234!","name":"Test Admin","role":0}')
# 201 = creado; 400/409 = email ya registrado (re-ejecución del script)
if [[ "$STATUS" == "201" || "$STATUS" == "409" || "$STATUS" == "400" ]]; then
    echo -e "${GREEN}[PASS]${NC} POST /api/auth/register  (HTTP $STATUS)"
    PASS=$((PASS + 1))
else
    assert "POST /api/auth/register" "201" "$STATUS"
fi

STATUS=$(req POST "$BASE/api/auth/login" \
    --json '{"email":"testrun@tattostudio.com","password":"Test1234!"}')
assert "POST /api/auth/login" "200" "$STATUS"
TOKEN=$(extract "token")

if [[ -z "$TOKEN" ]]; then
    echo -e "${RED}No se pudo obtener token JWT. Abortando.${NC}"
    exit 1
fi

# ─── 2. ARTISTS ─────────────────────────────────────────────────────────────

echo -e "\n${YELLOW}══════ ARTISTS ══════${NC}"

STATUS=$(req GET "$BASE/api/artists")
assert "GET /api/artists (anon)" "200" "$STATUS"

STATUS=$(req POST "$BASE/api/artists" \
    --token "$TOKEN" \
    --json '{"name":"Luna García","specialty":"Blackwork","commissionPercentage":20}')
assert "POST /api/artists" "201" "$STATUS"
ARTIST_ID=$(extract "id")

# ─── 3. CLIENTS ─────────────────────────────────────────────────────────────

echo -e "\n${YELLOW}══════ CLIENTS ══════${NC}"

UNIQUE="$(date +%s)"

STATUS=$(req POST "$BASE/api/clients" \
    --token "$TOKEN" \
    --json "{\"name\":\"Carlos Ruiz\",\"phone\":\"611987654\",\"email\":\"carlos_${UNIQUE}@test.com\",\"birthDate\":\"1985-03-20\",\"medicalNotes\":null}")
assert "POST /api/clients" "201" "$STATUS"
CLIENT_ID=$(extract "id")

STATUS=$(req GET "$BASE/api/clients" --token "$TOKEN")
assert "GET /api/clients" "200" "$STATUS"

STATUS=$(req GET "$BASE/api/clients/$CLIENT_ID" --token "$TOKEN")
assert "GET /api/clients/{id}" "200" "$STATUS"

STATUS=$(req PUT "$BASE/api/clients/$CLIENT_ID" \
    --token "$TOKEN" \
    --json "{\"name\":\"Carlos Ruiz (upd)\",\"phone\":\"611987654\",\"email\":\"carlos_${UNIQUE}@test.com\",\"birthDate\":\"1985-03-20\",\"medicalNotes\":\"Alergia al níquel\"}")
assert "PUT /api/clients/{id}" "200" "$STATUS"

# ─── 4. APPOINTMENTS ────────────────────────────────────────────────────────

echo -e "\n${YELLOW}══════ APPOINTMENTS ══════${NC}"

STATUS=$(req POST "$BASE/api/appointments" \
    --token "$TOKEN" \
    --json "{\"clientId\":\"$CLIENT_ID\",\"artistId\":\"$ARTIST_ID\",\"dateTime\":\"2026-08-15T10:00:00\",\"durationHours\":3,\"depositAmount\":50}")
assert "POST /api/appointments" "201" "$STATUS"
APT_ID=$(extract "id")

STATUS=$(req GET "$BASE/api/appointments/$APT_ID" --token "$TOKEN")
assert "GET /api/appointments/{id}" "200" "$STATUS"

STATUS=$(req GET "$BASE/api/appointments/calendar?from=2026-08-01&to=2026-08-31" --token "$TOKEN")
assert "GET /api/appointments/calendar" "200" "$STATUS"

STATUS=$(req GET "$BASE/api/appointments/$APT_ID/qr" --token "$TOKEN")
assert "GET /api/appointments/{id}/qr" "200" "$STATUS"
CONSENT_TOKEN=$(extract "token")

STATUS=$(req PUT "$BASE/api/appointments/$APT_ID/confirm-deposit" \
    --token "$TOKEN" \
    --json '{"depositAmount":50}')
assert "PUT /api/appointments/{id}/confirm-deposit" "200" "$STATUS"

echo -n 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==' \
    | base64 -d > /tmp/tattostudio_test_img.png
STATUS=$(req POST "$BASE/api/appointments/$APT_ID/media" \
    --token "$TOKEN" \
    --form "file=@/tmp/tattostudio_test_img.png;type=image/png" \
    --form "mediaType=0")
assert "POST /api/appointments/{id}/media" "201" "$STATUS"
MEDIA_ID=$(extract "id")

# ─── 5. CONSENTS ────────────────────────────────────────────────────────────
# Hay que firmar el consentimiento ANTES de completar la cita (REG-04-XX)

echo -e "\n${YELLOW}══════ CONSENTS (público) ══════${NC}"

STATUS=$(req GET "$BASE/api/public/consents/$CONSENT_TOKEN")
assert "GET /api/public/consents/{token}" "200" "$STATUS"

SIG="iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg=="
STATUS=$(req POST "$BASE/api/public/consents/$CONSENT_TOKEN/sign" \
    --json "{\"signatureBase64\":\"$SIG\"}")
assert "POST /api/public/consents/{token}/sign" "200" "$STATUS"

# ─── Volvemos a appointments: completar y borrar media ───────────────────────

echo -e "\n${YELLOW}══════ APPOINTMENTS (complete + media delete) ══════${NC}"

STATUS=$(req POST "$BASE/api/appointments/$APT_ID/complete" \
    --token "$TOKEN" \
    --json '{"stockItems":[{"stockItemName":"Tinta negra","quantity":2}]}')
assert "POST /api/appointments/{id}/complete" "200" "$STATUS"

STATUS=$(req DELETE "$BASE/api/appointments/$APT_ID/media/$MEDIA_ID" --token "$TOKEN")
assert "DELETE /api/appointments/{id}/media/{mediaId}" "204" "$STATUS"

# ─── 6. PAYMENTS ────────────────────────────────────────────────────────────

echo -e "\n${YELLOW}══════ PAYMENTS ══════${NC}"

STATUS=$(req POST "$BASE/api/payments" \
    --token "$TOKEN" \
    --json "{\"appointmentId\":\"$APT_ID\",\"amount\":200,\"type\":0,\"method\":0}")
assert "POST /api/payments" "201" "$STATUS"
PAY_ID=$(extract "id")

# ─── 7. STOCK ───────────────────────────────────────────────────────────────

echo -e "\n${YELLOW}══════ STOCK ══════${NC}"

STATUS=$(req POST "$BASE/api/stock" \
    --token "$TOKEN" \
    --json '{"name":"Tinta negra","currentQuantity":50,"minThreshold":10}')
assert "POST /api/stock" "201" "$STATUS"

STATUS=$(req GET "$BASE/api/stock/low-inventory" --token "$TOKEN")
assert "GET /api/stock/low-inventory" "200" "$STATUS"

# ─── 8. BILLING ─────────────────────────────────────────────────────────────

echo -e "\n${YELLOW}══════ BILLING ══════${NC}"

STATUS=$(req POST "$BASE/api/billing/invoices" \
    --token "$TOKEN" \
    --json "{\"paymentId\":\"$PAY_ID\"}")
assert "POST /api/billing/invoices" "201" "$STATUS"
INV_ID=$(extract "id")

STATUS=$(req GET "$BASE/api/billing/invoices/$INV_ID/verifactu-status" --token "$TOKEN")
assert "GET /api/billing/invoices/{id}/verifactu-status" "200" "$STATUS"

# ─── 9. DASHBOARD ───────────────────────────────────────────────────────────

echo -e "\n${YELLOW}══════ DASHBOARD ══════${NC}"

STATUS=$(req GET "$BASE/api/dashboard/summary?year=2026&month=6" --token "$TOKEN")
assert "GET /api/dashboard/summary" "200" "$STATUS"

# ─── 10. INTEGRATIONS ───────────────────────────────────────────────────────

echo -e "\n${YELLOW}══════ INTEGRATIONS ══════${NC}"

STATUS=$(req POST "$BASE/api/integrations/google-auth" \
    --token "$TOKEN" \
    --json '{"accessToken":"fake_access","refreshToken":"fake_refresh","expiresAt":"2026-07-23T12:00:00+00:00"}')
assert "POST /api/integrations/google-auth" "200" "$STATUS"

# ─── Resumen ────────────────────────────────────────────────────────────────

echo ""
echo "════════════════════════════════════════"
echo -e "  ${GREEN}PASS: $PASS${NC}   ${RED}FAIL: $FAIL${NC}   TOTAL: $((PASS + FAIL))"
echo "════════════════════════════════════════"

[[ $FAIL -eq 0 ]] && exit 0 || exit 1
