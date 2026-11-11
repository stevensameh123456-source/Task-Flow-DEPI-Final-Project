
// ==========================================
// TASKFLOW - PROFILE PAGE JAVASCRIPT
// ==========================================

document.addEventListener("DOMContentLoaded", function () {

    // ELEMENTS
    const profileForm = document.getElementById("profileForm");
    const passwordForm = document.getElementById("passwordForm");

    const fullName = document.getElementById("fullName");
    const email = document.getElementById("email");
    const jobTitle = document.getElementById("jobTitle");
    const locationInput = document.getElementById("location");

    const sidebarName = document.getElementById("sidebarName");
    const sidebarEmail = document.getElementById("sidebarEmail");
    const sidebarJob = document.getElementById("sidebarJob");
    const sidebarLocation = document.getElementById("sidebarLocation");

    const avatarInput = document.getElementById("avatarInput");
    const profileAvatar = document.getElementById("profileAvatar");

    const bio = document.getElementById("bio");
    const bioCount = document.getElementById("bioCount");

    const toast = document.getElementById("profileToast");
    const toastMessage = document.getElementById("toastMessage");

    let toastTimer;


    // SHOW NOTIFICATION
    function showToast(message, type = "success") {

        if (!toast || !toastMessage) return;

        toastMessage.textContent = message;

        const icon = toast.querySelector("i");

        if (icon) {
            icon.className = type === "error"
                ? "fa-solid fa-circle-exclamation"
                : "fa-solid fa-circle-check";

            icon.style.color = type === "error"
                ? "#ed6464"
                : "#2fbd88";
        }

        toast.classList.add("show");

        clearTimeout(toastTimer);

        toastTimer = setTimeout(function () {
            toast.classList.remove("show");
        }, 3000);
    }


    // UPDATE SIDEBAR INFORMATION
    function updateSidebar() {

        if (sidebarName && fullName) {
            sidebarName.textContent = fullName.value || "Your Name";
        }

        if (sidebarEmail && email) {
            sidebarEmail.textContent = email.value || "Your Email";
        }

        if (sidebarJob && jobTitle) {
            sidebarJob.textContent = jobTitle.value || "Your Job Title";
        }

        if (sidebarLocation && locationInput) {
            sidebarLocation.textContent =
                locationInput.value || "Your Location";
        }
    }


    // PROFILE TABS
    const tabs = document.querySelectorAll(".profile-tab");
    const tabContents = document.querySelectorAll(
        ".profile-tab-content"
    );

    tabs.forEach(function (tab) {

        tab.addEventListener("click", function () {

            const target = this.dataset.tab;

            tabs.forEach(function (item) {
                item.classList.remove("active");
            });

            tabContents.forEach(function (content) {
                content.classList.remove("active");
            });

            this.classList.add("active");

            const targetContent = document.getElementById(target);

            if (targetContent) {
                targetContent.classList.add("active");
            }

        });

    });


    // LIVE PROFILE PREVIEW
    [
        fullName,
        email,
        jobTitle,
        locationInput
    ].forEach(function (input) {

        if (!input) return;

        input.addEventListener("input", updateSidebar);

    });


    // AVATAR PREVIEW
    if (avatarInput && profileAvatar) {

        avatarInput.addEventListener("change", function () {

            const file = this.files[0];

            if (!file) return;

            if (!file.type.startsWith("image/")) {
                showToast("Please select a valid image.", "error");
                this.value = "";
                return;
            }

            const maxSize = 5 * 1024 * 1024;

            if (file.size > maxSize) {
                showToast("Image size must be less than 5 MB.", "error");
                this.value = "";
                return;
            }

            const reader = new FileReader();

            reader.onload = function (event) {
                profileAvatar.src = event.target.result;
            };

            reader.readAsDataURL(file);

        });

    }


    // BIO CHARACTER COUNTER
    function updateBioCount() {

        if (!bio || !bioCount) return;

        bioCount.textContent = bio.value.length;

    }

    if (bio) {
        bio.addEventListener("input", updateBioCount);
        updateBioCount();
    }


    // SAVE PROFILE - FRONTEND DEMO ONLY
    if (profileForm) {

        profileForm.addEventListener("submit", function (event) {

            event.preventDefault();

            if (!profileForm.reportValidity()) return;

            updateSidebar();

            showToast(
                "Profile preview updated. Connect the form to your backend to save changes."
            );

        });

    }


    // RESET PROFILE FORM
    const resetButton = document.getElementById("resetProfile");

    if (resetButton && profileForm) {

        const originalAvatar = profileAvatar
            ? profileAvatar.src
            : "";

        resetButton.addEventListener("click", function () {

            profileForm.reset();

            if (profileAvatar) {
                profileAvatar.src = originalAvatar;
            }

            if (avatarInput) {
                avatarInput.value = "";
            }

            updateSidebar();
            updateBioCount();

            showToast("Profile changes have been reset.");

        });

    }


    // SHOW / HIDE PASSWORD
    const passwordToggles = document.querySelectorAll(
        ".password-toggle"
    );

    passwordToggles.forEach(function (button) {

        button.addEventListener("click", function () {

            const targetId = this.dataset.target;
            const input = document.getElementById(targetId);

            if (!input) return;

            const isPassword = input.type === "password";

            input.type = isPassword ? "text" : "password";

            const icon = this.querySelector("i");

            if (icon) {
                icon.classList.toggle("fa-eye", !isPassword);
                icon.classList.toggle("fa-eye-slash", isPassword);
            }

        });

    });


    // PASSWORD FORM - FRONTEND DEMO ONLY
    if (passwordForm) {

        passwordForm.addEventListener("submit", function (event) {

            event.preventDefault();

            if (!passwordForm.reportValidity()) return;

            const currentPassword = document.getElementById(
                "currentPassword"
            ).value;

            const newPassword = document.getElementById(
                "newPassword"
            ).value;

            const confirmPassword = document.getElementById(
                "confirmPassword"
            ).value;

            if (newPassword.length < 8) {
                showToast(
                    "New password must be at least 8 characters.",
                    "error"
                );
                return;
            }

            if (currentPassword === newPassword) {
                showToast(
                    "New password must be different from current password.",
                    "error"
                );
                return;
            }

            if (newPassword !== confirmPassword) {
                showToast(
                    "Passwords do not match.",
                    "error"
                );
                return;
            }

            showToast(
                "Password validation successful. Backend integration is required to update it."
            );

            passwordForm.reset();

        });

    }


    // INITIALIZE
    updateSidebar();

});
