let timer;
let resendCount = 0;

// Start the timer
function startTimer(timeLeft) {
    $('#timer').html(timeLeft);

    timer = setInterval(() => {
        timeLeft--;
        localStorage.setItem('remainingTime', timeLeft); // Save remaining time to localStorage
        $('#timer').html(timeLeft);

        if (timeLeft <= 0) {
            clearInterval(timer);
            localStorage.removeItem('remainingTime'); // Clear saved timer on timeout
            document.getElementById('resendLink').style.pointerEvents = 'auto';
            document.getElementById('resendLink').style.color = 'blue';

            // Disable OTP input fields after time is up
            const otpFields = document.querySelectorAll('.otp-input');
            otpFields.forEach(field => {
                field.disabled = true;
            });
        }
    }, 1000);
}

// Function to move focus to the next input field
function moveToNext(current, nextId) {
    if (current.value.length == current.maxLength) {
        if (nextId) {
            document.getElementById(nextId).focus();
        }
    }
}

// Resend OTP functionality
function resendOtp() {
    // Hide error message if visible
    const errorMessageDiv = document.getElementById('errorMessage');
    errorMessageDiv.style.display = 'none';

    resendCount++; // Increment resendCount on each attempt

    // Save the updated resendCount to localStorage
    localStorage.setItem('resendCount', resendCount);
    const resendLink = document.getElementById('resendLink');
    const messageContainer = document.getElementById('messageContainer');

    if (resendCount > 3) {
        messageContainer.textContent = 'You have reached the maximum number of resend attempts. Redirecting to login';
        messageContainer.style.color = 'red';
        messageContainer.style.display = 'block';

        setTimeout(() => {
            fetch('/Login/LogoutAsync', { method: 'GET' })
                .then(() => {
                    window.location.href = '/Login';
                })
                .catch(err => console.error('Error clearing session:', err));
        }, 2000);

        return;
    }

    cleanotpbox();
    resendLink.style.pointerEvents = 'none';
    resendLink.style.color = 'gray';
    messageContainer.style.display = 'none';

    fetch('/Login/resendOtp', { method: 'POST' })
        .then(response => {
            if (response.ok) {
                messageContainer.textContent = 'OTP has been resent to your registered email.';
                messageContainer.style.color = 'green';
                messageContainer.style.display = 'block';

                const otpFields = document.querySelectorAll('.otp-input');
                otpFields.forEach(field => {
                    field.disabled = false;
                });

                localStorage.setItem('remainingTime', totalTime); // Reset the timer
                clearInterval(timer); // Clear the existing timer
                startTimer(totalTime); // Start new timer
            } else {
                messageContainer.textContent = 'Failed to resend OTP.';
                messageContainer.style.color = 'red';
                messageContainer.style.display = 'block';
                resendLink.style.pointerEvents = 'auto';
                resendLink.style.color = 'blue';
            }
        })
        .catch(() => {
            messageContainer.textContent = 'An error occurred. Please try again.';
            messageContainer.style.color = 'red';
            messageContainer.style.display = 'block';
        });
}


// Function to handle backspace behavior
function handleBackspace(event, currentId, previousId) {
    if (event.key === 'Backspace') {
        const currentInput = document.getElementById(currentId);
        const previousInput = document.getElementById(previousId);

        if (currentInput.value === "") {
            previousInput.focus();
        }
    }
}

// Verify OTP functionality
document.getElementById("verifyBtn").addEventListener("click", function (event) {
    event.preventDefault()
});
function verifyOtp() {
    const otpInputs = [
        document.getElementById('otp1').value,
        document.getElementById('otp2').value,
        document.getElementById('otp3').value,
        document.getElementById('otp4').value,
        document.getElementById('otp5').value,
        document.getElementById('otp6').value
    ];

    const errorMessageDiv = document.getElementById('errorMessage');
    errorMessageDiv.style.display = 'none';

    if (otpInputs.some(input => input.trim() === '')) {
        errorMessageDiv.querySelector('p').textContent = "Please enter the OTP";
        errorMessageDiv.style.display = 'block';
        return;
    }

    const otp = otpInputs.join('');
    const data = { OTP: otp };
    document.getElementById('load').style.visibility = "visible";
    fetch('/Login/VerifyOtp', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data)
    })
        .then(response => response.json())
        .then(responseData => {
            if (responseData.success) {
                localStorage.removeItem('resendCount');
                errorMessageDiv.style.display = 'none';
                localStorage.removeItem('remainingTime'); // Clear saved timer on success
                window.location.href = responseData.redirectUrl;
            } else {
                cleanotpbox();
                errorMessageDiv.querySelector('p').textContent = responseData.message || "Invalid OTP. Please try again.";
                errorMessageDiv.style.display = 'block';
                document.getElementById('load').style.visibility = "hidden";
                document.getElementById("otp1").focus();
            }
        })
        .catch(error => {
            console.error("An error occurred while verifying OTP:", error);
            errorMessageDiv.querySelector('p').textContent = "An error occurred. Please try again.";
            errorMessageDiv.style.display = 'block';
            document.getElementById('load').style.visibility = "hidden";
        });
}

function cleanotpbox() {
    $("#otp1").val('');
    $("#otp2").val('');
    $("#otp3").val('');
    $("#otp4").val('');
    $("#otp5").val('');
    $("#otp6").val('');
}
 