# DAPR: publish and subscribe messages example
Example: Distributed Application Runtime(DAPR) Publish and Subscriber messages
+ Using .NET7
+ Using RabbitMQ on local
+ Using Container App, Dapr Component and AzureServiceBus in Azure
+ CI/CD: GitHub Action 

### Requirements
----------------
+ Install [Dapr](https://docs.dapr.io/getting-started/)
+ Install .NET7

### Structures
----------------
+ Producer: using the API sends messages
+ Consumer: receive messages from pubSubName and Topic
+ Messages: create message model
+ ComponentsLocal: `pubsub.yaml` is config your message bus (RabbitMQ)
+ Components: `pubsub.yaml` is config your message bus (Azure Service Bus)

### Usages

+ docker-compose:
    + [RabbitMQ](http://localhost:15672)
    + [Consumer](http://localhost:5000/swagger)
    + [Producer](http://localhost:5011/swagger)

+ kind:
    + [Dapr](https://dapr.assistdevops.com/overview)
    + [RabbitMQ](https://rabbitmq.assistdevops.com)
    + [Consumer](https://consumer.assistdevops.com/swagger)
    + [Producer](https://producer.assistdevops.com/swagger)

```
kubectl logs $(kubectl get pods -l app=consumer-app --output jsonpath='{.items[0].metadata.name}') --since 5m -f 
kubectl logs $(kubectl get pods -l app=producer-app --output jsonpath='{.items[0].metadata.name}') --since 5m -f 

```    

+ Start RabbitMQ:
    ```
    cd DemoMicroservices
    docker-compose up
    ```
+ Using RabbitMQ:
    Set PubSubName="pubsub"

+ Start Consumer(app-id: checkout)
    ```
    cd DemoMicroservices\Consumer
    dapr run --app-id checkout --components-path ../../ComponentsLocal --app-port 5000 --dapr-http-port 3500 --dapr-grpc-port 60002 dotnet run
    ```
    The /checkout endpoint matches the route defined in the subscriptions and this is where Dapr will send all topic messages to.
    It will be created a queue with name: checkout-orders

+ Start Producer(app-id: orderprocessing)
    ```
    cd DemoMicroservices\Producer
    dapr run --app-id orderprocessing --app-port 5011 --dapr-http-port 3501 --dapr-grpc-port 60001 --components-path ../../ComponentsLocal dotnet run
    ```

+ Send a message from Producer to Consumer
    ```
    http://localhost:5011/order
    {
        "OrderAmount":12.7,
        "OrderNumber":"Num18291",
        "OrderDate":"2021-12-21"
    }
    ```
### References
--------------
+ [Dapr Pub Sub ASP Net Core integration](https://yourazurecoach.com/2019/12/27/exploring-dapr-pub-sub-part-2-asp-net-core-integration/)
+ [Dapr publish subcriber for net developers](https://docs.microsoft.com/en-us/dotnet/architecture/dapr-for-net-developers/publish-subscribe)
+ [Publish a message and subscribe to a topic](https://docs.dapr.io/developing-applications/building-blocks/pubsub/howto-publish-subscribe/)
