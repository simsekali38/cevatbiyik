"use strict";
const summary = document.querySelector(".validation-summary-errors");
const success = document.getElementById("contact-success");
if (summary) summary.focus();
else if (success) success.focus();

// Only a functional preference state is shared; authorization remains in a protected HttpOnly cookie.
const consentPanel = document.querySelector("[data-consent-state]");
if (consentPanel) {
    const state = consentPanel.dataset.consentState;
    try { localStorage.setItem("cevat-consent-state", state); } catch (_) { /* optional browser storage */ }
    window.addEventListener("storage", event => {
        if (event.key === "cevat-consent-state" && event.newValue !== state) location.reload();
    });
}
