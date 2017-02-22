## WebHooks Documentation
1. WebHook registration:
    Make a **POST** request to the following route: **/api/webhooks/registrations**
    
    With JSON data:
    
    ```json
    {
      "WebHookUri":"https://yourUrl.com/yourApiEndpoint?NoEcho=true",
      "Filters":["product/updated"]
     }
     ```
     
     **NOTE:** If the **NoEcho** parameter is not passed a **GET** request will be made to the **WebHookUri** endpoint,
     passing an **echo** parameter. The endpoint should return the **echo** parameter with a status code 200.
     This is done in order to verify that the endpoint is valid and that it is configured to work with the webhooks.
     
     
2. Get the available WebHookFilters aka. WebHook events
     
        Make a **GET** request to the following route: **/api/webhooks/filters**
        
3. Get all WebHooks (the webhooks are per authenticated customer)
     
        Make a **GET** request to the following route: **/api/webhooks/registrations**
        
4. Get a specific WebHook
     
        Make a **GET** request to the following route: **/api/webhooks/registrations/{webhookid}**
         
5. Update a WebHook
     
        Make a **PUT** request to the following route: **/api/webhooks/registrations/{webhookid}**
        
        The JSON should contain at least the WebHook Id, WebHookUri and Filters.
      
6. Delete a specific WebHook

         Make a **DELETE** request to the following route: **/api/webhooks/registrations/{webhookid}** 
         
7. Delete all WebHooks

          Make a **DELETE** request to the following route: **/api/webhooks/registrations**
          
          
### Webhook Verification
    
   In order to verify that the webhook is sent from the nopCommerce api you should check the request header for a ** ms-signature ** key. The key should contain a similar value: ** sha256=21349127391293**
    
   In order to verify the webhook you should use the following code:
    
   ```cs
        private bool VerifyWebhook(string data, string hashedHeader, string webhookSecret)
        {
            byte[] secret = Encoding.UTF8.GetBytes(webhookSecret);
            using (var hasher = new HMACSHA256(secret))
            {
                byte[] encodedData = Encoding.UTF8.GetBytes(data);
                byte[] sha256 = hasher.ComputeHash(encodedData);
                string hashedData = string.Format(CultureInfo.InvariantCulture, "sha256={0}", EncodingUtilities.ToHex(sha256));

                return hashedHeader.Equals(hashedData);
            }
        }
   ```
