importScripts('https://www.gstatic.com/firebasejs/9.0.0/firebase-app-compat.js');
importScripts('https://www.gstatic.com/firebasejs/9.0.0/firebase-messaging-compat.js');

// If you would like to customize notifications that are received in the
// background (Web app is closed or not in browser focus) then you should
// implement this optional method.
// Keep in mind that FCM will still show notification messages automatically 
// and you should use data messages for custom notifications.
// For more info see: 
// https://firebase.google.com/docs/cloud-messaging/concept-options
const firebaseConfig = {
    apiKey: "AIzaSyBMEAYJ3P-ZifnqEH84u8ScfdlncVkuUzo",
    authDomain: "anielconnect-4ec81.firebaseapp.com",
    projectId: "anielconnect-4ec81",
    storageBucket: "anielconnect-4ec81.appspot.com",
    messagingSenderId: "844266081578",
    appId: "1:844266081578:web:4b76903dedd10b3c2d2253"
};

firebase.initializeApp(firebaseConfig);

const messaging = firebase.messaging();

console.log('Service Worker Firebase');

messaging.onBackgroundMessage(function (payload) {
    console.log('[firebase-messaging-sw.js] Received background message ', payload);
    // Customize notification here
    const notificationTitle = 'Background Message Title';
    const notificationOptions = {
        body: 'Background Message body.',
        //icon: '/firebase-logo.png'
    };
    
    self.registration.showNotification(notificationTitle,
        notificationOptions);
});
