document.addEventListener("DOMContentLoaded", function () {
    const themeToggleButton = document.getElementById("theme-toggle");
    const body = document.body;

    let isDarkMode = localStorage.getItem("dark-mode") === "enabled";

    function updateTheme() {
        if (isDarkMode) {
            body.classList.add("dark-mode");
            body.classList.remove("light-mode");
            themeToggleButton.textContent = "☀️ حالت روشن";
        } else {
            body.classList.add("light-mode");
            body.classList.remove("dark-mode");
            themeToggleButton.textContent = "🌙 حالت تاریک";
        }
    }

    themeToggleButton.addEventListener("click", function () {
        isDarkMode = !isDarkMode;
        localStorage.setItem("dark-mode", isDarkMode ? "enabled" : "disabled");
        updateTheme();
    });

    updateTheme();
});
