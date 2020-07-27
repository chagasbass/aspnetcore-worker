# aspnetcore-worker
poc de uma api Rest que envia dados para um fila no RabbitMQ e um  worker que faz a leitura dessa fila usando com aspnet core.
O RabbitMQ foi usado no docker

Executando o Rabbit
docker run -d --hostname rabbitserver --name rabbitmq-server -p 15672:15672 -p 5672:5672 rabbitmq:3-management
