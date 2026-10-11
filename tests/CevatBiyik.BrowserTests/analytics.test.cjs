"use strict";
const test = require("node:test");
const assert = require("node:assert/strict");
const vm = require("node:vm");
const fs = require("node:fs");
const path = require("node:path");
const code = fs.readFileSync(path.join(__dirname, "../../src/CevatBiyik.Web/wwwroot/js/analytics.js"), "utf8");
const siteCode = fs.readFileSync(path.join(__dirname, "../../src/CevatBiyik.Web/wwwroot/js/site.js"), "utf8");
function run(provider = "gtm", id = "GTM-TEST123", recorded = false, render = true) {
    const handlers = {}, storage = [], elements = [], formHandlers = {};
    const script = render ? { dataset: { provider, id, contactRecorded: String(recorded) }, nonce: "test-nonce" } : null;
    const panel = { dataset: { consentState: "v1:allow" } };
    const window = { addEventListener: (type, cb) => { (handlers[type] ||= []).push(cb); } };
    const context = { window,
        location: { origin: "https://example.test", pathname: "/iletisim", search: "?email=SECRET_EMAIL", reload: () => { context.reloaded = true; } },
        localStorage: { setItem: (key, value) => storage.push([key, value]) },
        document: {
            querySelector: selector => selector === "[data-consent-state]" ? panel : selector.startsWith("script") ? script : null,
            getElementById: () => null,
            createElement: () => ({}), head: { appendChild: element => elements.push(element) },
            addEventListener: (type, cb) => { (handlers[type] ||= []).push(cb); },
            querySelectorAll: selector => selector === "[data-open-dialog]" ? [] : [{ addEventListener: (type, cb) => { formHandlers[type] = cb; } }]
        }, console
    };
    vm.runInNewContext(code, context);
    return { context, window, handlers, elements, storage, formHandlers, panel,
        click(href, eventName) { handlers.click?.forEach(cb => cb({ target: { closest: () => ({ getAttribute: () => href, dataset: { analyticsEvent: eventName } }) } })); },
        events() { return JSON.parse(JSON.stringify(window.dataLayer || [])); }
    };
}
test("no consent-rendered script or invalid ID produces no remote request/dataLayer", () => {
    for (const s of [run("gtm", "GTM-TEST123", false, false), run("gtm", "GTM-<script>"), run("ga4", "not-ga")]) {
        assert.equal(s.elements.length, 0); assert.equal(s.window.dataLayer, undefined);
    }
});
test("GTM loader copies nonce and only whitelisted event names without PII", () => {
    const s = run(); assert.equal(s.elements[0].nonce, "test-nonce");
    assert.equal(s.elements[0].src, "https://www.googletagmanager.com/gtm.js?id=GTM-TEST123");
    s.click("tel:+900000000", undefined); s.click("mailto:SECRET_EMAIL", undefined);
    s.click("https://wa.me/90000000?text=SECRET_MESSAGE"); s.click("/", "map_click"); s.click("/", "cta_click");
    s.click("/", "SECRET_EVENT");
    const data = s.events(); const events = data.filter(x => x.event).map(x => x.event);
    assert.deepEqual(events, ["gtm.js", "phone_click", "email_click", "whatsapp_click", "map_click", "cta_click"]);
    assert.ok(!JSON.stringify(data).includes("SECRET")); assert.ok(!JSON.stringify(data).includes("90000000"));
});
test("contact_submit fires once from server's successful save marker only", () => {
    assert.equal(run().events().filter(x => x.event === "contact_submit").length, 0);
    assert.equal(run("gtm", "GTM-TEST123", true).events().filter(x => x.event === "contact_submit").length, 1);
});
test("GA4 sends no automatic page view, advertising or query/referrer payload", () => {
    const s = run("ga4", "G-TEST123", true); s.click("mailto:SECRET_EMAIL");
    assert.equal(s.elements[0].nonce, "test-nonce");
    const data = s.events(); const config = data.find(x => x[0] === "config");
    assert.equal(config[2].send_page_view, false); assert.equal(config[2].page_location, "https://example.test/iletisim");
    assert.equal(data[0][2].ad_storage, "denied"); assert.equal(data[0][2].ad_user_data, "denied");
    assert.equal(data[0][2].ad_personalization, "denied"); assert.ok(!JSON.stringify(data).includes("SECRET"));
    assert.ok(data.some(x => x[0] === "event" && x[1] === "contact_submit"));
});
test("withdrawal immediately stops hooks; cross-tab preference change stops old page", () => {
    const s = run(); const before = s.events().length;
    s.formHandlers.submit(); s.click("tel:SECRET"); assert.equal(s.events().length, before);
    const other = run(); const otherBefore = other.events().length;
    other.handlers.storage.forEach(cb => cb({ key: "cevat-consent-state", newValue: "v1:deny" }));
    other.click("tel:SECRET"); assert.equal(other.events().length, otherBefore);
});
test("functional consent state is broadcast after navigation and reloads differing tabs", () => {
    const s = run(); vm.runInNewContext(siteCode, s.context);
    assert.deepEqual(s.storage, [["cevat-consent-state", "v1:allow"]]);
    s.handlers.storage.forEach(cb => cb({ key: "cevat-consent-state", newValue: "v1:allow" }));
    assert.equal(s.context.reloaded, undefined);
    s.handlers.storage.forEach(cb => cb({ key: "cevat-consent-state", newValue: "v1:deny" }));
    assert.equal(s.context.reloaded, true);
});
