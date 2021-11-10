#import "TCGBAuthPlugin.h"
#import "DelegateManager.h"
#import <Gamebase/Gamebase.h>
#import "TCGBJsonUtil.h"
#import "NativeMessage.h"
#import "TCGBPluginData.h"
#import "EngineMessage.h"
#import "TCGBViewControllerManager.h"

#define AUTH_API_LOGIN                                  @"gamebase://login"
#define AUTH_API_LOGIN_ADDITIONAL_INFO                  @"gamebase://loginWithAdditionalInfo"
#define AUTH_API_LOGIN_CREDENTIAL_INFO                  @"gamebase://loginWithCredentialInfo"
#define AUTH_API_LOGIN_FOR_LAST_LOGGED_IN_PROVIDER      @"gamebase://loginForLastLoggedInProvider"
#define AUTH_API_LOGOUT                                 @"gamebase://logout"
#define AUTH_API_ADD_MAPPING                            @"gamebase://addMapping"
#define AUTH_API_ADD_MAPPING_CREDENTIAL_INFO            @"gamebase://addMappingWithCredentialInfo"
#define AUTH_API_ADD_MAPPING_ADDITIONAL_INFO            @"gamebase://addMappingWithAdditionalInfo"
#define AUTH_API_ADD_MAPPING_FORCIBLY                   @"gamebase://addMappingForcibly"
#define AUTH_API_ADD_MAPPING_FORCIBLY_CREDENTIAL_INFO   @"gamebase://addMappingForciblyWithCredentialInfo"
#define AUTH_API_ADD_MAPPING_FORCIBLY_ADDITIONAL_INFO   @"gamebase://addMappingForciblyWithAdditionalInfo"
#define AUTH_API_REMOVE_MAPPING                         @"gamebase://removeMapping"
#define AUTH_API_WITHDRAW_ACCOUT                        @"gamebase://withdraw"
#define AUTH_API_WITHDRAW_IMMEDIATELY_ACCOUT            @"gamebase://withdrawImmediately"
#define AUTH_API_REQUEST_TEMPORARY_WITHDRAWAL_ACCOUT    @"gamebase://requestTemporaryWithdrawal"
#define AUTH_API_CANCEL_TEMPORARY_WITHDRAWAL_ACCOUT     @"gamebase://cancelTemporaryWithdrawal"
#define AUTH_API_ISSUE_TRANSFER_ACCOUNT                 @"gamebase://issueTransferAccount"
#define AUTH_API_QUERY_TRANSFER_ACCOUNT                 @"gamebase://queryTransferAccount"
#define AUTH_API_RENEW_TRANSFER_ACCOUNT                 @"gamebase://renewTransferAccount"
#define AUTH_API_TRANSFER_ACCOUNT_WITH_IDP_LOGIN        @"gamebase://transferAccountWithIdPLogin"
#define AUTH_API_GET_AUTH_MAPPING_LIST                  @"gamebase://getAuthMappingList"
#define AUTH_API_GET_AUTH_PROVIDER_USERID               @"gamebase://getAuthProviderUserID"
#define AUTH_API_GET_AUTH_PROVIDER_ACCESSTOKEN          @"gamebase://getAuthProviderAccessToken"
#define AUTH_API_GET_AUTH_PROVIDER_PROFILE              @"gamebase://getAuthProviderProfile"
#define AUTH_API_GET_BAN_INFO                           @"gamebase://getBanInfo"

// transfer account
#define RENEWAL_MODE_TYPE                               @"renewalModeType"
#define RENEWAL_MODE_TYPE_MANUAL                        @"MANUAL"
#define RENEWAL_MODE_TYPE_AUTO                          @"AUTO"
#define RENEWAL_TARGET_TYPE                             @"renewalTargetType"
#define RENEWAL_TARGET_TYPE_PASSWORD                    0
#define RENEWAL_TARGET_TYPE_ID_PASSWORD                 1
#define ACCOUNT_ID                                      @"accountId"
#define ACCOUNT_PASSWORD                                @"accountPassword"    

@implementation TCGBAuthPlugin

- (instancetype)init {
    if ((self = [super init]) == nil) {
        return nil;
    }
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:AUTH_API_LOGIN target:self selector:@selector(login:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:AUTH_API_LOGIN_ADDITIONAL_INFO target:self selector:@selector(loginWithAdditionalInfo:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:AUTH_API_LOGIN_CREDENTIAL_INFO target:self selector:@selector(loginWithCredentialInfo:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:AUTH_API_LOGIN_FOR_LAST_LOGGED_IN_PROVIDER target:self selector:@selector(loginForLastLoggedInProvider:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:AUTH_API_LOGOUT target:self selector:@selector(logout:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:AUTH_API_ADD_MAPPING target:self selector:@selector(addMapping:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:AUTH_API_ADD_MAPPING_CREDENTIAL_INFO target:self selector:@selector(addMappingWithCredentialInfo:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:AUTH_API_ADD_MAPPING_ADDITIONAL_INFO target:self selector:@selector(addMappingWithAdditionalInfo:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:AUTH_API_ADD_MAPPING_FORCIBLY target:self selector:@selector(addMappingForcibly:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:AUTH_API_ADD_MAPPING_FORCIBLY_CREDENTIAL_INFO target:self selector:@selector(addMappingForciblyWithCredentialInfo:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:AUTH_API_ADD_MAPPING_FORCIBLY_ADDITIONAL_INFO target:self selector:@selector(addMappingForciblyWithAdditionalInfo:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:AUTH_API_REMOVE_MAPPING target:self selector:@selector(removeMapping:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:AUTH_API_WITHDRAW_ACCOUT target:self selector:@selector(withdraw:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:AUTH_API_WITHDRAW_IMMEDIATELY_ACCOUT target:self selector:@selector(withdrawImmediately:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:AUTH_API_REQUEST_TEMPORARY_WITHDRAWAL_ACCOUT target:self selector:@selector(requestTemporaryWithdrawal:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:AUTH_API_CANCEL_TEMPORARY_WITHDRAWAL_ACCOUT target:self selector:@selector(cancelTemporaryWithdrawal:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:AUTH_API_ISSUE_TRANSFER_ACCOUNT target:self selector:@selector(issueTransferAccount:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:AUTH_API_QUERY_TRANSFER_ACCOUNT target:self selector:@selector(queryTransferAccount:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:AUTH_API_RENEW_TRANSFER_ACCOUNT target:self selector:@selector(renewTransferAccount:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:AUTH_API_TRANSFER_ACCOUNT_WITH_IDP_LOGIN target:self selector:@selector(transferAccountWithIdPLogin:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:AUTH_API_GET_AUTH_MAPPING_LIST target:self selector:@selector(getAuthMappingList:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:AUTH_API_GET_AUTH_PROVIDER_USERID target:self selector:@selector(getAuthProviderUserID:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:AUTH_API_GET_AUTH_PROVIDER_ACCESSTOKEN target:self selector:@selector(getAuthProviderAccessToken:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:AUTH_API_GET_AUTH_PROVIDER_PROFILE target:self selector:@selector(getAuthProviderProfile:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:AUTH_API_GET_BAN_INFO target:self selector:@selector(getBanInfo:)];
    
    return self;
}

-(void)login:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSDictionary* convertedDic = [message.jsonData JSONDictionary];
    
    [TCGBGamebase loginWithType:convertedDic[@"providerName"] viewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(id authToken, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[authToken description] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);        
    }];
}

-(void)loginWithAdditionalInfo:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSDictionary* convertedDic = [message.jsonData JSONDictionary];
    
    [TCGBGamebase loginWithType:convertedDic[@"providerName"] additionalInfo:convertedDic[@"additionalInfo"] viewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(id authToken, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[authToken JSONString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);        
    }];
}

-(void)loginWithCredentialInfo:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSDictionary* convertedDic = [message.jsonData JSONDictionary];
    
    [TCGBGamebase loginWithCredential:convertedDic viewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(id authToken, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[authToken JSONString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);        
    }];
}

-(void)loginForLastLoggedInProvider:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    [TCGBGamebase loginForLastLoggedInProviderWithViewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(id authToken, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[authToken JSONString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);        
    }];
}

-(void)logout:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    [TCGBGamebase logoutWithViewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:nil extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);        
    }];
}

-(void)addMapping:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSDictionary* convertedDic = [message.jsonData JSONDictionary];
    [TCGBGamebase addMappingWithType:convertedDic[@"providerName"] viewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(id authToken, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[authToken JSONString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);        
    }];
}

-(void)addMappingWithCredentialInfo:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSDictionary* convertedDic = [message.jsonData JSONDictionary];
    
    [TCGBGamebase addMappingWithCredential:convertedDic viewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(id authToken, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[authToken JSONString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)addMappingWithAdditionalInfo:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSDictionary* convertedDic = [message.jsonData JSONDictionary];
    [TCGBGamebase addMappingWithType:convertedDic[@"providerName"] additionalInfo:convertedDic[@"additionalInfo"] viewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(id authToken, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[authToken JSONString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);        
    }];
}

-(void)addMappingForcibly:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSDictionary* convertedDic = [message.jsonData JSONDictionary];
    [TCGBGamebase addMappingForciblyWithType:convertedDic[@"providerName"] forcingMappingKey:convertedDic[@"forcingMappingKey"] viewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(id authToken, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[authToken JSONString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);        
    }];
}

-(void)addMappingForciblyWithCredentialInfo:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSDictionary* convertedDic = [message.jsonData JSONDictionary];
    
    [TCGBGamebase addMappingWithCredential:convertedDic[@"credentialInfo"] forcingMappingKey:convertedDic[@"forcingMappingKey"] viewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(id authToken, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[authToken JSONString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)addMappingForciblyWithAdditionalInfo:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSDictionary* convertedDic = [message.jsonData JSONDictionary];
    [TCGBGamebase addMappingForciblyWithType:convertedDic[@"providerName"] forcingMappingKey:convertedDic[@"forcingMappingKey"] additionalInfo:convertedDic[@"additionalInfo"] viewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(id authToken, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[authToken JSONString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);        
    }];
}

-(void)removeMapping:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSDictionary* convertedDic = [message.jsonData JSONDictionary];
    [TCGBGamebase removeMappingWithType:convertedDic[@"providerName"] viewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:nil extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);        
    }];
}

-(void)withdraw:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    [TCGBGamebase withdrawWithViewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:nil extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);        
    }];
}

-(void)withdrawImmediately:(TCGBPluginData*)pluginData {
	EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
	
	[TCGBGamebase withdrawImmediatelyWithViewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:nil extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);        
    }];
}

-(void)requestTemporaryWithdrawal:(TCGBPluginData*)pluginData {
	EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
	
	[TCGBGamebase requestTemporaryWithdrawalWithViewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(TCGBTemporaryWithdrawalInfo* temporaryWithdrawalInfo, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[temporaryWithdrawalInfo JSONString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);        
    }];
}

-(void)cancelTemporaryWithdrawal:(TCGBPluginData*)pluginData {
	EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
	
	[TCGBGamebase cancelTemporaryWithdrawalWithViewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:nil extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);        
    }];
}

-(void)issueTransferAccount:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];

    [TCGBGamebase issueTransferAccountWithCompletion:^(TCGBTransferAccountInfo *transferAccount, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[transferAccount description] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)queryTransferAccount:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    [TCGBGamebase queryTransferAccountWithCompletion:^(TCGBTransferAccountInfo *transferAccount, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[transferAccount description] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)renewTransferAccount:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSDictionary* convertedDic = [message.jsonData JSONDictionary];
    NSString* renewalModeType = convertedDic[RENEWAL_MODE_TYPE];
    NSInteger renewalTargetType = [convertedDic[RENEWAL_TARGET_TYPE] integerValue];
    
    TCGBTransferAccountRenewConfiguration* configuration = nil;
    
    if([renewalModeType caseInsensitiveCompare:RENEWAL_MODE_TYPE_MANUAL] == NSOrderedSame) {
        if(renewalTargetType == RENEWAL_TARGET_TYPE_PASSWORD) {
            configuration = [TCGBTransferAccountRenewConfiguration manualRenewConfigurationWithAccountPassword:convertedDic[ACCOUNT_PASSWORD]];
        }
        else if(renewalTargetType == RENEWAL_TARGET_TYPE_ID_PASSWORD){
            configuration = [TCGBTransferAccountRenewConfiguration manualRenewConfigurationWithAccountId:convertedDic[ACCOUNT_ID] accountPassword:convertedDic[ACCOUNT_PASSWORD]];
        }
    }
    else {
        if(renewalTargetType == RENEWAL_TARGET_TYPE_PASSWORD) {
            configuration = [TCGBTransferAccountRenewConfiguration autoRenewConfigurationWithRenewalTarget:TCGBTransferAccountRenewalTargetTypePassword];
        }
        else if(renewalTargetType == RENEWAL_TARGET_TYPE_ID_PASSWORD){
            configuration = [TCGBTransferAccountRenewConfiguration autoRenewConfigurationWithRenewalTarget:TCGBTransferAccountRenewalTargetTypeIdPassword];
        }
    }
    
    [TCGBGamebase renewTransferAccountWithConfiguration:configuration completion:^(TCGBTransferAccountInfo *transferAccount, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[transferAccount description] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)transferAccountWithIdPLogin:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSDictionary* convertedDic = [message.jsonData JSONDictionary];
    
    [TCGBGamebase transferAccountWithIdPLoginWithAccountId:convertedDic[ACCOUNT_ID] accountPassword:convertedDic[ACCOUNT_PASSWORD] completion:^(TCGBAuthToken *authToken, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[authToken description] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(NSString*)getAuthMappingList:(TCGBPluginData*)pluginData {
    NSString* result = [[TCGBGamebase authMappingList] JSONStringFromArray];
    return result;
}

-(NSString*)getAuthProviderUserID:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSString* result = [TCGBGamebase authProviderUserIDWithIDPCode:message.jsonData];
    return result;
}

-(NSString*)getAuthProviderAccessToken:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSString* result = [TCGBGamebase authProviderAccessTokenWithIDPCode:message.jsonData];
    return result;
}

-(NSString*)getAuthProviderProfile:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    TCGBAuthProviderProfile* profil = [TCGBGamebase authProviderProfileWithIDPCode:message.jsonData];
    if(profil == nil)
    {
        return nil;
    }
    NSString* result = [profil description];
    return result;
}

-(NSString*)getBanInfo:(TCGBPluginData*)pluginData {
    TCGBBanInfo* info = [TCGBGamebase banInfo];
    if(info == nil)
    {
        return nil;
    }
    NSString* result = [info description];
    return result;
}
@end

