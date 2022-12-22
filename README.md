# simple-graphql-c-sharp

This is a simple `GraphQL` based `C#` application that demonstrates some API calls and connection with databases such as postrgresql and redis. 

## Starting up

Running `docker compose up -d` or `docker-compose up -d` should set up the server. 

One thing to keep an eye on is the PORT address for the server. Locally the HotChocolate GraphQL server can start on `5000` on OSX while inside ASPNET 7.0 docker image the server can start at port `80`. Either way this would be found in the startup log of the api container or docker compose logs.

`notifi_api       | Now listening on: http://[::]:80`

Update the docker compose port forwarding accordingly in the `docker-compose.yaml` file.

## Api Queries

### Simple Greeting

Query:

```
query {
    greeting {
        message
    }
}
```

Expected Output:

```
{
    "data": {
        "greeting": {
            "message": "Hello World"
        }
    }
}
```

### Random JWT Token 

Returns a random JWT token

Query:
``` 
query {
    authToken {
        token
    }
}

```


Expected Output:

```
{
    "data": {
        "authToken": {
            "token": <Generated JWT token>
        }
    }
}
```

### Get Total Number of Api Hits 
This returns the total number any of the endpoints been called.

Query:

```
query {
    totalApiHits {
        hits
    }
}
```
Example Output:

```
{
    "data": {
        "totalApiHits": {
            "hits": 3
        }
    }
}
```

### Check Redis Connection
This checks whether redis server is running or not.

Query:

``` 
query {
    redisConnection {
        active
    }
}
```
Example Output:

``` 
{
    "data": {
        "redisConnection": {
            "active": true
        }
    }
}
```
