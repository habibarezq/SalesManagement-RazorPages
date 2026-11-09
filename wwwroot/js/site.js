// Any <form data-confirm="Are you sure?"> asks before submitting.
document.addEventListener("submit", function (e) {
    var message = e.target.getAttribute("data-confirm");
    if (message && !window.confirm(message)) {
        e.preventDefault();
    }
});
