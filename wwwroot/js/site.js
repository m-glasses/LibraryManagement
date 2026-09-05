// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {

    const sidebar = document.getElementById("app-sidebar");
    const toggle = document.getElementById("sidebar-toggle");

    if (!sidebar || !toggle) {
        return;
    }

    toggle.addEventListener("click", function () {

        const isOpen = sidebar.classList.toggle("show");

        toggle.setAttribute("aria-expanded", isOpen);

    });

});