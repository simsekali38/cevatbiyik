"use strict";
(() => {
    const script = document.querySelector('script[data-provider][data-id]');
    if (!script) return;
    const id = script.dataset.id;
    const provider = script.dataset.provider;
    if (!(provider === "gtm" && /^GTM-[A-Z0-9]{4,20}$/.test(id)) && !(provider === "ga4" && /^G-[A-Z0-9]{4,20}$/.test(id))) return;
    window.dataLayer = window.dataLayer || [];
    const gtag = function () { window.dataLayer.push(arguments); };
    // Basic consent mode: this file is rendered only after opt-in. Advertising remains denied.
    gtag("consent", "default", { analytics_storage: "granted", ad_storage: "denied", ad_user_data: "denied", ad_personalization: "denied" });
    const cleanPage = location.origin + location.pathname;
    if (provider === "gtm") {
        window.dataLayer.push({ "gtm.start": Date.now(), event: "gtm.js", page_location: cleanPage, page_referrer: "" });
    } else {
        gtag("js", new Date());
        gtag("config", id, { send_page_view: false, page_location: cleanPage, page_referrer: "", allow_google_signals: false, allow_ad_personalization_signals: false });
    }
    const remote = document.createElement("script");
    remote.async = true;
    remote.nonce = script.nonce;
    remote.src = "https://www.googletagmanager.com/" + (provider === "gtm" ? "gtm.js?id=" : "gtag/js?id=") + encodeURIComponent(id);
    document.head.appendChild(remote);
    const events = new Set(["phone_click", "whatsapp_click", "email_click", "contact_submit", "map_click", "cta_click"]);
    let enabled = true;
    const emit = (name) => {
        if (!enabled || !events.has(name)) return;
        if (provider === "ga4") gtag("event", name, { page_location: cleanPage, page_referrer: "" });
        else window.dataLayer.push({ event: name });
    };
    if (script.dataset.contactRecorded === "true") emit("contact_submit");
    document.addEventListener("click", (event) => {
        const anchor = event.target.closest && event.target.closest("a");
        if (!anchor) return;
        const href = anchor.getAttribute("href") || "";
        const name = href.startsWith("tel:") ? "phone_click" : href.startsWith("mailto:") ? "email_click" :
            /^https:\/\/(wa\.me|api\.whatsapp\.com)\//.test(href) ? "whatsapp_click" : anchor.dataset.analyticsEvent;
        emit(name); // Never emit href, text, query, message, address or other form data.
    });
    document.querySelectorAll("[data-consent-form]").forEach(form => form.addEventListener("submit", () => {
        enabled = false;
        // Navigation completes before site.js broadcasts the committed consent choice.
    }));
    window.addEventListener("storage", event => { if (event.key === "cevat-consent-state") { enabled = false; } });
})();
