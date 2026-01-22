$('#loginForm').submit(function (e) {
    e.preventDefault();

    var username = $('#username').val();
    var password = $('#password').val();

    // Add spinner to the button
    var $loginButton = $('#loginForm button[type="submit"]');
    $loginButton.prop('disabled', true); // Disable the button
    $loginButton.html('<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Logging in...');

    $.ajax({
        url: '/Account/Login',
        type: 'POST',
        data: {
            Username: username, // Match model property names in the controller
            Password: password
        },
        success: function (response) {
            if (response.success) {
                if (response.requiresPasswordChange) {
                    $('#loginModal').modal('hide');
                    $('#changePasswordModal').modal('show');
                } else {

                    // Redirect to the dashboard 
                    alert(response.message);
                    window.location.href = response.redirectUrl;
                }
            } else {
                alert(response.message);
            }
        },
        error: function () {
            alert("Login failed. Please try again.");
        },
        complete: function () {
            // Reset button state
            $loginButton.prop('disabled', false);
            $loginButton.html('<i class="fas fa-sign-in-alt me-2"></i>Login');
        }
    });
}); 


//let loginAttempts = 0; // Counter for login attempts
//const maxAttempts = 3; // Maximum allowed attempts
//let countdownInterval; // To store the interval ID for countdown

//// Check if the maximum attempts have been reached
//if (loginAttempts >= maxAttempts) {
//    alert("Too many failed login attempts. Please wait for the countdown to complete.");
//    return;
//}

////else part
//loginAttempts++;
//alert(response.message || `Login failed. You have ${maxAttempts - loginAttempts} attempt(s) left.`);
//if (loginAttempts >= maxAttempts) {
//    startCountdown($loginButton);


//// Function to start the countdown timer
//function startCountdown($loginButton) {
//    let countdown = 10; // Countdown duration in seconds

//    // Disable the button during the countdown
//    $loginButton.prop('disabled', true);
//    $loginButton.html(`Please wait ${countdown} seconds...`);

//    countdownInterval = setInterval(function () {
//        countdown--;
//        $loginButton.html(`Please wait ${countdown} seconds...`);

//        if (countdown <= 0) {
//            clearInterval(countdownInterval);
//            loginAttempts = 0; // Reset attempts after the countdown
//            $loginButton.prop('disabled', false);
//            $loginButton.html('<i class="fas fa-sign-in-alt me-2"></i>Login');
//        }
//    }, 1000);
//}