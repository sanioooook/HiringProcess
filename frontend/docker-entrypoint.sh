#!/bin/sh
set -e

FRONTEND_URL="${APP_FRONTEND_URL:-http://localhost}"

# Replace __FRONTEND_URL__ placeholder in static files
for file in \
  /usr/share/nginx/html/index.html \
  /usr/share/nginx/html/sitemap.xml \
  /usr/share/nginx/html/robots.txt; do
  sed -i "s|__FRONTEND_URL__|${FRONTEND_URL}|g" "$file"
done

exec nginx -g "daemon off;"
