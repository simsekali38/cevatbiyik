"use strict";
const summary = document.querySelector(".validation-summary-errors");
const success = document.getElementById("contact-success");
if (summary) summary.focus();
else if (success) success.focus();

const privacyDialog = document.getElementById("kvkk-notice");
if (privacyDialog) document.querySelectorAll('a[href="/kvkk"]').forEach(link => { link.dataset.openDialog = privacyDialog.id; });
const activeDialogOpeners = new WeakMap();
const boundDialogs = new WeakSet();
document.querySelectorAll("[data-open-dialog]").forEach(opener => {
    const dialog = document.getElementById(opener.dataset.openDialog);
    if (!dialog || typeof dialog.showModal !== "function") return;
    opener.setAttribute("aria-haspopup", "dialog"); opener.setAttribute("aria-controls", dialog.id);
    opener.addEventListener("click", event => { event.preventDefault(); activeDialogOpeners.set(dialog, opener); dialog.showModal(); dialog.querySelector("[data-close-dialog]")?.focus(); });
    if (boundDialogs.has(dialog)) return;
    boundDialogs.add(dialog);
    dialog.querySelector("[data-close-dialog]")?.addEventListener("click", () => dialog.close());
    dialog.addEventListener("close", () => activeDialogOpeners.get(dialog)?.focus());
    dialog.addEventListener("keydown", event => {
        if (event.key !== "Tab") return;
        const targets = [...dialog.querySelectorAll("a[href],button,input,select,textarea")].filter(node => !node.disabled && node.getClientRects().length);
        const first = targets[0], last = targets.at(-1);
        if (event.shiftKey && document.activeElement === first) { event.preventDefault(); last.focus(); }
        else if (!event.shiftKey && document.activeElement === last) { event.preventDefault(); first.focus(); }
    });
});

// Only a functional preference state is shared; authorization remains in a protected HttpOnly cookie.
const consentPanel = document.querySelector("[data-consent-state]");
if (consentPanel) {
    const state = consentPanel.dataset.consentState;
    try { localStorage.setItem("cevat-consent-state", state); } catch (_) { /* optional browser storage */ }
    window.addEventListener("storage", event => {
        if (event.key === "cevat-consent-state" && event.newValue !== state) location.reload();
    });
}
