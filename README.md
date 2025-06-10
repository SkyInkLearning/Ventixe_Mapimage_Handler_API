# Mapimage Handler API

## Purpose and thinking:

This is one part of the map image for the ventixe site. This API takes in an image and returns the url from the blob storage on azure.

Check the other part for more information. ( https://github.com/SkyInkLearning/Ventixe_Map_API )


## Sequence diagram plantuml

<img src="https://github.com/user-attachments/assets/84937f84-3cfa-40ca-acd6-83e8442aff60" width="400">

# Postman:

## Authentication:

All requests to this API require an API-Key to be passed in the header under "X-API-KEY". 

Invalid requests will be met with:

```json
{
    "success": false,
    "error": "Invalid api-key or api-key is missing."
}
```

## POST and PUT: 


```json

```


### Created By:

https://github.com/SimonR-prog

