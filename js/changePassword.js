$(document).ready(function () {
    $('#changePasswordForm').submit(function (e) {
        e.preventDefault(); // Prevent the default form submission

        // Retrieve form data
        var newPassword = $('#newPassword').val();
        var confirmPassword = $('#confirmPassword').val();

        // Basic validation: Check if passwords match
        if (newPassword !== confirmPassword) {
            alert("Passwords do not match.");
            return;
        }

        // Add spinner to the button and disable it
        var $submitButton = $(this).find("button[type='submit']");
        var $spinner = $('#changePasswordSpinner');
        $submitButton.prop('disabled', true);
        $spinner.show();

        // AJAX request
        $.ajax({
            url: '/Account/ChangePassword', // The URL where the form data will be sent
            type: 'POST',
            data: {
                newPassword: newPassword,
                confirmPassword: confirmPassword
            },
            success: function (response) {
                if (response.success) {
                    alert("Password changed successfully.");
                    $('#changePasswordModal').modal('hide'); // Hide the modal

                } else {
                    alert(response.message); // Show error message from the server
                }
            },
            error: function () {
                alert("An error occurred. Please try again.");
            },
            complete: function () {
                // Reset button and spinner state
                $submitButton.prop('disabled', false);
                $spinner.hide();
            }
        });
    });
});
