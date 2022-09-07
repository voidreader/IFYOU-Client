//
//  NPWebViewDataTypes.h
//  Native Plugins
//
//  Created by Ashwin kumar on 22/01/19.
//  Copyright (c) 2019 Voxel Busters Interactive LLP. All rights reserved.
//

#import "NPDefines.h"

// callback signatures
typedef void (*WebViewNativeCallback)(NPIntPtr nativePtr, const char* error);

typedef void (*WebViewRunJavaScriptNativeCallback)(NPIntPtr nativePtr, const char* result, const char* error, NPIntPtr tagPtr);

typedef void (*WebViewURLSchemeMatchFoundNativeCallback)(NPIntPtr nativePtr, const char* url);
