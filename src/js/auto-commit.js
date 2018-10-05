window.onload = function () {
    var focus = localStorage.getItem('focus');
    if (focus) {
        document.getElementsByName(focus)[0].focus();
    }
};
function changed(input) {
    input.form.submit();
}
function gotFocus(input) {
    localStorage.setItem('focus', input.name);
}
function lostFocus(input) {
    localStorage.removeItem('focus');
}