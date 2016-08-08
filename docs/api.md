# Authentication with Github

## Login with Github Account

### Current login Process

1. Login link on the home page
1. Click "Github" to be re-directed to Github login screen
1. Authorize the app for scope of "public_repo"
 
# Github API

After authorized with github:

1. GET `api/github/token`: Token is saved on authorizing with Github. It is avaiable at: `/api/github/token` to get token
 * Token can be saved somewhere for test purpose so that developers do not need to log-out and login again to gain new token.
 * if return null means that user need to logout and login with github again to fetch a token.

1. GET `api/github/user`: get current authorized github user details

1. GET `api/github/issues`: get all issues of the repository `/repos/angular-bbs/q-and-a`

1. POST `api/github/issues`: create new issue on behalf of the authorized user, body:  
    ```
    {
        "title": "title text",
        "body": "body text",
        "token": "TOKEN"
    }
    ```
    * title: issue title, required
    * body: issue body
    * token: valid access token, required. 

# Github Secret

1. Read client id and secret: Goto orgnisation `Angular-bbs`, `Settings` -> `OAuth Application` -> `angular-bbs`

2. Set secret: 
```
{
    "GitHub:ClientId": "%clientid%",
    "GitHub:ClientSecret": "%secret%"
}
```
Alternatively, for test purpose, hardcode ClientId and secret in `Startup.cs`, replace:  

* `Configuration["GitHub:ClientId"]` with `%ClientId%` 
* `Configuration["GitHub:ClientSecret"]` with `%Secret%` 