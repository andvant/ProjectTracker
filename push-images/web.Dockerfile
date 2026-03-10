FROM nginx:1.28-alpine
EXPOSE 80

# artifacts are built in workflow build.yaml
COPY dist/ /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

CMD ["nginx", "-g", "daemon off;"]
