FROM mcr.microsoft.com/dotnet/sdk:8.0
ENV DB_VOL=/var/michat-db
ENV JWT_SECRET=MyCustomSecret
ENV CERT_PATH=/var/michat-cert/michat.pfx
ENV CERT_PASSWORD=1234
ENV STATIC_PATH=/var/michat/www-root
WORKDIR /var/michat
LABEL author=neyronbite
COPY . .
COPY ./www-root $STATIC_PATH
RUN chmod +x CliChat
RUN chmod +x entry.sh
EXPOSE 443
CMD ./entry.sh