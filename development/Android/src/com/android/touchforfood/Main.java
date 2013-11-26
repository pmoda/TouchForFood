package com.android.touchforfood;


import java.util.List;

import com.android.touchforfood.R;
import android.app.Activity;
import android.content.Intent;
import android.content.pm.FeatureInfo;
import android.nfc.NdefMessage;
import android.nfc.NdefRecord;
import android.nfc.NfcAdapter;
import android.os.Bundle;
import android.os.Message;
import android.os.Parcelable;
import android.util.Log;
import android.view.KeyEvent;
import android.view.Window;
import android.view.WindowManager;
import android.webkit.WebChromeClient;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.Toast;

public class Main extends Activity{
	final static String TAG = "com.android.touchforfood";
	WebView mWebView;
	Activity activity;
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		requestWindowFeature(Window.FEATURE_INDETERMINATE_PROGRESS);
		requestWindowFeature(Window.FEATURE_PROGRESS);
		this.getWindow().setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_HIDDEN);
		activity = this;
		setContentView(R.layout.web_viewer);

		mWebView = (WebView) findViewById(R.id.webview);

		mWebView.setWebViewClient(new WebViewClient() {
			@Override
			public boolean shouldOverrideUrlLoading(WebView view, String url) {
				view.loadUrl(url);
				return true;
			}
		});
		mWebView.getSettings().setJavaScriptEnabled(true);
		mWebView.getSettings().setSupportMultipleWindows(true);
		mWebView.setWebChromeClient(new WebChromeClient() {
			@Override
			public boolean onCreateWindow(WebView view, boolean dialog, boolean userGesture, Message resultMsg) {
				Toast.makeText(getApplicationContext(), "OnCreateWindow", Toast.LENGTH_LONG).show();
				return true;
			}
			
			@Override
			public void onProgressChanged(WebView view, int progress){
				setProgress(progress * 100);
	              if(progress == 100) {
	                  setProgressBarIndeterminateVisibility(false);
	                  setProgressBarVisibility(false);
	               }
			}
		});
		mWebView.loadUrl("http://ryanweb.dyndns.info:2431/");
	}
	
	@Override
	public void onBackPressed(){
		if(!mWebView.canGoBack()) super.onBackPressed();
			mWebView.goBack();
	}
	
    @Override
    protected void onResume() {
        super.onResume();
        Intent intent = getIntent();
        String action = intent.getAction();
        if (Intent.ACTION_VIEW.equals(action) ) {
        	String url = intent.toUri(0);
        	int end = url.indexOf("#");
        	url = url.substring(0, end);
            mWebView.loadUrl(url);
        }
    }
}

