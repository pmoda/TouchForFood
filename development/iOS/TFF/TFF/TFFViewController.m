//
//  TFFViewController.m
//  TFF
//
//  Created by Cristian Asenjo on 2013-01-29.
//  Copyright (c) 2013 CloudNine. All rights reserved.
//

#import "TFFViewController.h"
#import "AppDelegate.h"

@interface TFFViewController ()

@end

@implementation TFFViewController


@synthesize webView;
@synthesize restaurantURL;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
    }
    return self;
}

- (void)viewWillAppear:(BOOL)animated
{
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    // Load webcontent in the webView
    AppDelegate *delegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    restaurantURL = delegate.scannedURL;

    NSURL *url = [NSURL URLWithString:restaurantURL];
    NSURLRequest *request = [NSURLRequest requestWithURL:url];
    [webView loadRequest:request];
}

- (void) showAlert:(NSString *)title :(NSString *)message {
    
    UIAlertView *alert = [[UIAlertView alloc]
                          
                          initWithTitle:title
                          message:message
                          delegate:nil
                          cancelButtonTitle:@"OK"
                          otherButtonTitles:nil];
    
    [alert show];
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

@end
