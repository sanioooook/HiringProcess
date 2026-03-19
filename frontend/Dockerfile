# Build Angular app
FROM node:22-alpine AS build
WORKDIR /app

# Copy package files first (layer-cache friendly)
COPY package.json package-lock.json ./
RUN npm ci --prefer-offline

# Copy source and build
COPY . .
RUN npm run build -- --configuration=production

# Serve with Nginx
FROM nginx:1.27-alpine AS runtime

# Remove default nginx config
RUN rm /etc/nginx/conf.d/default.conf

# Add custom nginx config and entrypoint
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh

# Copy built Angular app
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html

EXPOSE 80

ENTRYPOINT ["/docker-entrypoint.sh"]
