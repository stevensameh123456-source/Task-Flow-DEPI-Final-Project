
(function () {
    'use strict';

    // =========================================
    // Elements
    // =========================================

    const form = document.getElementById('registerForm');

    const fullName = document.getElementById('FullName');
    const email = document.getElementById('Email');

    const password = document.getElementById('Password');
    const confirmPassword = document.getElementById('ConfirmPassword');

    const agreeTerms = document.getElementById('AgreeTerms');

    const registerBtn = document.getElementById('registerBtn');
    const themeBtn = document.getElementById('themeToggle');

    const strengthBar = document.getElementById('strengthBar');
    const strengthText = document.getElementById('strengthText');

    // =========================================
    // Theme Toggle
    // =========================================

    if (themeBtn) {

        themeBtn.addEventListener('click', function () {

            const root = document.documentElement;

            const currentTheme = root.getAttribute('data-bs-theme');

            const newTheme = currentTheme === 'dark'
                ? 'light'
                : 'dark';

            root.setAttribute('data-bs-theme', newTheme);

            try {
                localStorage.setItem('tf-theme', newTheme);
            } catch (error) {
                console.warn('Could not save theme preference.');
            }

            themeBtn.setAttribute(
                'aria-label',
                newTheme === 'dark'
                    ? 'Switch to light mode'
                    : 'Switch to dark mode'
            );

        });

    }

    // =========================================
    // Password Visibility
    // =========================================

    document.querySelectorAll('.tf-eye').forEach(function (button) {

        button.addEventListener('click', function () {

            const inputId = this.dataset.password;
            const input = document.getElementById(inputId);

            if (!input) return;

            const show = input.type === 'password';

            input.type = show ? 'text' : 'password';

            const icon = this.querySelector('i');

            if (icon) {

                icon.className = show
                    ? 'fa-regular fa-eye-slash'
                    : 'fa-regular fa-eye';

            }

            this.setAttribute(
                'aria-label',
                show ? 'Hide password' : 'Show password'
            );

        });

    });

    // =========================================
    // Error Handler
    // =========================================

    function setError(input, message) {

        if (!input) return !message;

        const errorBox = document.querySelector(
            '.tf-error[data-for="' + input.id + '"]'
        );

        if (message) {

            input.classList.add('is-invalid');

            if (errorBox) {
                errorBox.textContent = message;
            }

        } else {

            input.classList.remove('is-invalid');

            if (errorBox) {
                errorBox.textContent = '';
            }

        }

        return !message;

    }

    // =========================================
    // Full Name Validation
    // =========================================

    function validateFullName() {

        if (!fullName) return true;

        const value = fullName.value.trim();

        if (!value) {
            return setError(fullName, 'Full name is required.');
        }

        if (value.length < 3) {
            return setError(fullName, 'Name must be at least 3 characters.');
        }

        return setError(fullName, '');

    }

    // =========================================
    // Email Validation
    // =========================================

    function validateEmail() {

        if (!email) return true;

        const value = email.value.trim();

        if (!value) {
            return setError(email, 'Email address is required.');
        }

        const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (!emailPattern.test(value)) {
            return setError(email, 'Please enter a valid email address.');
        }

        return setError(email, '');

    }

    // =========================================
    // Password Strength
    // =========================================

    function updatePasswordStrength() {

        if (!password || !strengthBar || !strengthText) return;

        const value = password.value;

        if (!value) {

            strengthBar.style.width = '0';
            strengthBar.style.background = 'transparent';

            strengthText.textContent = 'Use at least 8 characters.';

            return;

        }

        let score = 0;

        if (value.length >= 8) score++;
        if (/[a-z]/.test(value)) score++;
        if (/[A-Z]/.test(value)) score++;
        if (/[0-9]/.test(value)) score++;
        if (/[^a-zA-Z0-9]/.test(value)) score++;

        const levels = [
            { width: '20%', color: '#ef4444', text: 'Very weak' },
            { width: '35%', color: '#f97316', text: 'Weak' },
            { width: '55%', color: '#eab308', text: 'Fair' },
            { width: '80%', color: '#84cc16', text: 'Good' },
            { width: '100%', color: '#22c55e', text: 'Strong' }
        ];

        const level = levels[score - 1];

        strengthBar.style.width = level.width;
        strengthBar.style.background = level.color;

        strengthText.textContent = 'Password strength: ' + level.text;

    }

    // =========================================
    // Password Validation
    // =========================================

    function validatePassword() {

        if (!password) return true;

        const value = password.value;

        if (!value) {
            return setError(password, 'Password is required.');
        }

        if (value.length < 8) {
            return setError(password, 'Password must be at least 8 characters.');
        }

        return setError(password, '');

    }

    // =========================================
    // Confirm Password Validation
    // =========================================

    function validateConfirmPassword() {

        if (!confirmPassword) return true;

        const value = confirmPassword.value;

        if (!value) {
            return setError(confirmPassword, 'Please confirm your password.');
        }

        if (value !== password.value) {
            return setError(confirmPassword, 'Passwords do not match.');
        }

        return setError(confirmPassword, '');

    }

    // =========================================
    // Terms Validation
    // =========================================

    function validateTerms() {

        if (!agreeTerms) return true;

        if (!agreeTerms.checked) {
            return setError(agreeTerms, 'You must accept the terms to continue.');
        }

        return setError(agreeTerms, '');

    }

    // =========================================
    // Live Validation
    // =========================================

    if (fullName) {

        fullName.addEventListener('blur', validateFullName);

        fullName.addEventListener('input', function () {

            if (fullName.classList.contains('is-invalid')) {
                validateFullName();
            }

        });

    }

    if (email) {

        email.addEventListener('blur', validateEmail);

        email.addEventListener('input', function () {

            if (email.classList.contains('is-invalid')) {
                validateEmail();
            }

        });

    }

    if (password) {

        password.addEventListener('input', function () {

            updatePasswordStrength();

            if (password.classList.contains('is-invalid')) {
                validatePassword();
            }

            if (confirmPassword && confirmPassword.value) {
                validateConfirmPassword();
            }

        });

        password.addEventListener('blur', validatePassword);

    }

    if (confirmPassword) {

        confirmPassword.addEventListener('input', function () {

            if (confirmPassword.classList.contains('is-invalid')) {
                validateConfirmPassword();
            }

        });

        confirmPassword.addEventListener('blur', validateConfirmPassword);

    }

    if (agreeTerms) {

        agreeTerms.addEventListener('change', validateTerms);

    }

    // =========================================
    // Form Submission
    // =========================================

    if (form) {

        form.addEventListener('submit', function (event) {

            const nameValid = validateFullName();
            const emailValid = validateEmail();
            const passwordValid = validatePassword();
            const confirmValid = validateConfirmPassword();
            const termsValid = validateTerms();

            if (
                !nameValid ||
                !emailValid ||
                !passwordValid ||
                !confirmValid ||
                !termsValid
            ) {

                event.preventDefault();

                if (!nameValid) {
                    fullName.focus();
                }
                else if (!emailValid) {
                    email.focus();
                }
                else if (!passwordValid) {
                    password.focus();
                }
                else if (!confirmValid) {
                    confirmPassword.focus();
                }
                else {
                    agreeTerms.focus();
                }

                return;

            }

            // Loading State

            if (registerBtn) {

                const btnText = registerBtn.querySelector('.btn-text');
                const btnLoading = registerBtn.querySelector('.btn-loading');

                if (btnText) {
                    btnText.classList.add('d-none');
                }

                if (btnLoading) {
                    btnLoading.classList.remove('d-none');
                }

                registerBtn.disabled = true;

            }

        });

    }

    // =========================================
    // Reset Button After Page Cache
    // =========================================

    window.addEventListener('pageshow', function () {

        if (!registerBtn) return;

        registerBtn.disabled = false;

        const btnText = registerBtn.querySelector('.btn-text');
        const btnLoading = registerBtn.querySelector('.btn-loading');

        if (btnText) {
            btnText.classList.remove('d-none');
        }

        if (btnLoading) {
            btnLoading.classList.add('d-none');
        }

    });

})();
