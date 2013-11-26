//
//  TFFViewController.h
//  TFF
//
//  Created by Cristian Asenjo on 2013-01-29.
//  Copyright (c) 2013 CloudNine. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface TFFViewController : UIViewController

// Declare a property (variable)
@property (nonatomic, strong) IBOutlet UIWebView *webView;
@property (nonatomic, strong) NSString *restaurantURL;
@end
