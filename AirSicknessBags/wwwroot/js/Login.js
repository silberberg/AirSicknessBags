function onSignIn(googleUser) {
    var profile = googleUser.getBasicProfile();
    //alert('ID: ' + profile.getId()); // Do not send to your backend! Use an ID token instead.
    //alert('Name: ' + profile.getName());
    //alert('Image URL: ' + profile.getImageUrl());
    //alert('Email: ' + profile.getEmail()); // This is null if the 'email' scope is not present.
}

document.onload = function displayHidden(googleUser) {
    var profile = googleUser.getBasicProfile();
    if (profile.getName() == 'fatpacking@gmail.com') {
        document.getElementById('EditPeopleButton').style.visibility = 'visible';
    }
    document.getElementById('EditPeopleButton').style.visibility = 'hidden';
}

function signOut() {
    var auth2 = gapi.auth2.getAuthInstance();
    auth2.signOut().then(function () {
        console.log('User signed out.');
    });
}
