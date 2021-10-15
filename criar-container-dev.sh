docker run -d \
    --name postgres \
    -e POSTGRES_PASSWORD=123@Mudar \
    -e PGDATA=/var/lib/postgresql/data/pgdata \
    -p 5432:5432 \
    postgres:alpine3.14  

docker run -d \
    --name rabbitmq \
    -p 5672:5672 \
    -p 15672:15672 \
    rabbitmq:3-management