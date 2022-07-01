# .net-core-Entity-framework-project

Small web-applciation which posses a functionality for creation of forum sections/ topics/ messages with attachments.

Application has a role management:

Admin:
1. CRUD access for all parts of the forum. 
2. Admin Can assign moderators for forum sections.

Moderators:
1. Can Edit Forum Sections assigned to them. 
2. CRUD to associated topics and messages.

Common registered users:
1. Can create topics and messages. 
2. CRUD access to self-created messages.

Unregistered users:
1. Can read through the forum.

Applciation can also function as a simple API:

API requests:
_______________________________________________________________

1. Get all forum sections:
URL: https://<website>/api/sections

<---------------------------------->
 
2. Add forum section:
URL: https://<website>/api/sections
Method: POST
Input data format (Request body): JSON
{
    "name" : string,
    "description" : string
}

<---------------------------------->

3. Edit forum section:
URL: https://<website>/api/sections/{id}
Method: PUT
Section Id - from URL parameter
Input data format (Request body): JSON
{
    "name" : string,
    "description" : string
}

<---------------------------------->

4. Get topics for section:
URL: https://<website>/api/sections/{id}/topics

<---------------------------------->

5. Add forum topic:
URL: https://<website>/api/sections/{id}/topics
Method: POST
Input data format (Request body): JSON
{
    "name" : string,
    "description" : string
}

<---------------------------------->

6. Delete forum topic:
URL: https://<website>/api/topics/{id}
Method: DELETE

<---------------------------------->

7.Edit forum topic:
URL: https://<website>/api/topics/{id}
Method: PUT
Topic Id - from URL parameter
{
    "name" : string,
    "description" : string
}

<---------------------------------->

8. Get topic messages
URL: https://<website>/api/topics/{id}/messages

<---------------------------------->

9. Add messages
URL: https://<website>/api/topics/{id}/messages
Method: POST
Topic Id for message creation - from URL parameter
Input data format (Request body): JSON
{
    "text" : string
}

<---------------------------------->

10. Edit message:
URL: https://<website>/api/messages/{id}
Method: PUT
Message Id - from URL parameter
{
    "text" : string
}

<---------------------------------->

11. Delete message:
URL: https://<website>/api/messages/{id}
Method: DELETE

<---------------------------------->




