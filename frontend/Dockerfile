FROM node:24-alpine AS build
WORKDIR /app
EXPOSE 80

COPY package*.json ./
RUN npm install

COPY vite.config.ts tsconfig*.json ./
COPY .env env.d.ts index.html ./
COPY src src/
RUN npm run build

FROM nginx:1.28-alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

CMD ["nginx", "-g", "daemon off;"]
