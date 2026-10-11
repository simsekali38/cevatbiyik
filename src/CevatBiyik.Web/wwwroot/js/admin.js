"use strict";
(() => {
    document.body.classList.remove("admin-no-js");
    const editors = [];
    const make = (tag, className, text) => {
        const node = document.createElement(tag);
        if (className) node.className = className;
        if (text !== undefined) node.textContent = text;
        return node;
    };
    const allowedLink = value => {
        if (!value || /[\u0000-\u001f\u007f\\]/.test(value) || value.startsWith("//")) return false;
        if (value.startsWith("/") || value.startsWith("#")) return true;
        try { const url = new URL(value); return ["https:", "http:"].includes(url.protocol) && !url.username && !url.password; }
        catch (_) { return false; }
    };
    const announce = (node, message) => { node.textContent = message; };
    const keepDialogFocus = dialog => dialog.addEventListener("keydown", event => {
        if (event.key !== "Tab") return;
        const targets = [...dialog.querySelectorAll('a[href], button, input, select, textarea, [tabindex]')]
            .filter(node => !node.disabled && node.tabIndex >= 0 && node.getClientRects().length);
        const first = targets[0], last = targets.at(-1);
        if (event.shiftKey && document.activeElement === first) { event.preventDefault(); last.focus(); }
        else if (!event.shiftKey && document.activeElement === last) { event.preventDefault(); first.focus(); }
        // Escape remains native and returns focus through the close handler.
    });

    if (window.Quill) document.querySelectorAll("textarea[data-rich-editor]").forEach((source, number) => {
        const required = source.required;
        const limit = source.maxLength > 0 ? source.maxLength : 100000;
        const wrapper = make("div", "admin-editor");
        const menubar = make("div", "admin-editor-menubar"); menubar.setAttribute("role", "group"); menubar.setAttribute("aria-label", "Editör menüsü");
        const toolbar = make("div", "admin-editor-toolbar");
        toolbar.setAttribute("role", "group"); toolbar.setAttribute("aria-label", "Metin biçimlendirme araçları");
        const host = make("div"); const footer = make("div", "admin-editor-footer");
        const count = make("p"); const help = make("p", "", "Tab ile araçlar ve form alanları arasında gezinin. Ctrl+B: kalın, Ctrl+I: italik.");
        help.id = source.id + "-editor-help";
        const error = make("p", "field-error"); error.setAttribute("role", "alert"); error.hidden = true;
        wrapper.append(menubar, toolbar, host, footer); footer.append(count, help);
        source.before(wrapper); source.after(error);
        const editor = new Quill(host, {theme: null, formats: ["header", "bold", "italic", "underline", "blockquote", "list", "link", "align"], modules: {toolbar: false, history: {userOnly: true}}});
        editor.root.setAttribute("role", "textbox"); editor.root.setAttribute("aria-multiline", "true");
        editor.root.setAttribute("aria-required", String(required));
        editor.root.setAttribute("aria-describedby", help.id);
        editor.root.id = source.id + "-editor";
        const label = document.querySelector(`label[for="${CSS.escape(source.id)}"]`);
        if (label) { label.htmlFor = editor.root.id; label.id ||= source.id + "-label"; editor.root.setAttribute("aria-labelledby", label.id); label.addEventListener("click", () => editor.focus()); }
        // The server tag helper has already sanitized HTML; legacy text is imported without interpreting markup.
        if (source.value.includes("<")) {
            const imported = new DOMParser().parseFromString(source.value, "text/html");
            imported.querySelectorAll("[class]").forEach(node => { node.className = node.className.replace(/^content-align-(center|right|justify)$/, "ql-align-$1"); });
            editor.setContents(editor.clipboard.convert({html: imported.body.innerHTML}), "silent");
        }
        else editor.setText(source.value, "silent");
        editor.getModule("history").clear();
        source.hidden = true; source.required = false;
        let selection = {index: 0, length: 0};

        // Let native Tab leave the editor, including from inside lists; never trap keyboard users.
        editor.root.addEventListener("keydown", event => { if (event.key === "Tab") event.stopImmediatePropagation(); }, true);
        const restoreSelection = () => { editor.focus(); editor.setSelection(selection.index, selection.length, "silent"); };
        const controls = [];
        const commands = new Map();
        const iconPaths = {
            "Geri al": "M9 6 4 11l5 5M4 11h9a6 6 0 0 1 6 6",
            "Yinele": "m15 6 5 5-5 5m5-5h-9a6 6 0 0 0-6 6",
            "Bağlantı": "m10 14 4-4m-6 5-2 2a4 4 0 0 1-6-6l4-4a4 4 0 0 1 6 0m4 2 2-2a4 4 0 0 1 6 6l-4 4a4 4 0 0 1-6 0",
            "Bağlantıyı kaldır": "m10 14 4-4M3 3l18 18M8 15l-2 2a4 4 0 0 1-6-6l4-4m12-2a4 4 0 0 1 6 6l-4 4",
            "Madde listesi": "M9 5h12M9 12h12M9 19h12M3 5h1M3 12h1M3 19h1",
            "Numaralı liste": "M9 5h12M9 12h12M9 19h12M3 3v4M2 3h1M2 11q3-2 3 0l-3 3h3M2 18h3l-2 2h2",
            "Alıntı": "M4 7h6v6H5v4H3v-6l1-4Zm11 0h6v6h-5v4h-2v-6l1-4Z",
            "Biçimi temizle": "M5 4h14M12 4l-3 16m5-2 6 4m0-4-6 4",
            "Genişlet": "M4 9V4h5m6 0h5v5M4 15v5h5m6 0h5v-5",
            "Önizleme": "M2 12s4-7 10-7 10 7 10 7-4 7-10 7S2 12 2 12Zm10-3a3 3 0 1 0 0 6 3 3 0 0 0 0-6",
            "Kaynak görünümü": "m8 6-6 6 6 6m8-12 6 6-6 6m-3-14-2 16",
            "Sola hizala": "M3 5h18M3 10h11M3 15h18M3 20h11",
            "Ortala": "M3 5h18M6 10h12M3 15h18M6 20h12",
            "Sağa hizala": "M3 5h18M10 10h11M3 15h18M10 20h11",
            "İki yana yasla": "M3 5h18M3 10h18M3 15h18M3 20h18"
        };
        const setIcon = (control, labelText) => {
            control.replaceChildren(); control.title = labelText; control.setAttribute("aria-label", labelText);
            if (["Kalın", "İtalik", "Altı çizili"].includes(labelText)) {
                const letter = make("span", "editor-letter " + ({"Kalın":"is-bold","İtalik":"is-italic","Altı çizili":"is-underlined"}[labelText]), {"Kalın":"B","İtalik":"I","Altı çizili":"U"}[labelText]);
                letter.setAttribute("aria-hidden", "true"); control.append(letter); return;
            }
            const svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
            svg.setAttribute("viewBox", "0 0 24 24"); svg.setAttribute("aria-hidden", "true"); svg.setAttribute("focusable", "false");
            const path = document.createElementNS(svg.namespaceURI, "path"); path.setAttribute("d", iconPaths[labelText] || iconPaths["Genişlet"]); svg.append(path); control.append(svg);
        };
        const divider = () => { const line = make("span", "admin-editor-divider"); line.setAttribute("aria-hidden", "true"); toolbar.append(line); };
        const menu = (labelText, entries) => {
            const details = make("details", "admin-editor-menu"); const summary = make("summary", "", labelText);
            const popup = make("div", "admin-editor-menu-items"); details.append(summary, popup); menubar.append(details);
            entries.forEach(([label, action]) => { const item = make("button", "", label); item.type = "button"; item.addEventListener("click", () => { details.open = false; action(); }); popup.append(item); });
            details.addEventListener("toggle", () => { if (details.open) menubar.querySelectorAll("details").forEach(other => { if (other !== details) other.open = false; }); });
            details.addEventListener("keydown", event => { if (event.key === "Escape") { details.open = false; summary.focus(); } });
            document.addEventListener("click", event => { if (!details.contains(event.target)) details.open = false; });
        };
        const button = (labelText, action, format, value = true) => {
            const control = make("button", "admin-editor-tool"); control.type = "button"; setIcon(control, labelText); commands.set(labelText, () => control.click());
            if (format) { control.setAttribute("aria-pressed", "false"); controls.push({control, format, value}); }
            control.addEventListener("mousedown", event => event.preventDefault());
            control.addEventListener("click", () => { restoreSelection(); action(); updateButtons(); }); toolbar.append(control); return control;
        };
        const formatSelect = make("select"); formatSelect.setAttribute("aria-label", "Paragraf biçimi");
        [["", "Paragraf"], ["2", "Başlık 2"], ["3", "Başlık 3"]].forEach(([value, text]) => { const option = make("option", "", text); option.value = value; formatSelect.append(option); });
        formatSelect.addEventListener("change", () => { restoreSelection(); editor.format("header", Number(formatSelect.value) || false, "user"); }); toolbar.append(formatSelect); divider();
        [ ["Kalın", "bold"], ["İtalik", "italic"], ["Altı çizili", "underline"], ["Alıntı", "blockquote"] ].forEach(([text, format]) => button(text, () => editor.format(format, !editor.getFormat()[format], "user"), format));
        divider();
        [["Sola hizala", false], ["Ortala", "center"], ["Sağa hizala", "right"], ["İki yana yasla", "justify"]].forEach(([text, value]) => button(text, () => editor.format("align", value, "user"), "align", value));
        divider();
        [["Madde listesi", "bullet"], ["Numaralı liste", "ordered"]].forEach(([text, value]) => button(text, () => editor.format("list", editor.getFormat().list === value ? false : value, "user"), "list", value));

        const dialog = make("dialog", "admin-link-dialog"); dialog.setAttribute("aria-labelledby", `link-title-${number}`);
        const dialogForm = make("form"); dialogForm.method = "dialog";
        const title = make("h2", "", "Bağlantı ekle"); title.id = `link-title-${number}`;
        const info = make("p", "form-text", "Seçili metne bağlantı eklenir. Metin seçilmediyse adres metin olarak eklenir.");
        const urlLabel = make("label", "form-label", "Bağlantı adresi"); const urlInput = make("input", "form-control");
        urlInput.id = `link-url-${number}`; urlInput.type = "text"; urlInput.inputMode = "url"; urlInput.required = true; urlInput.maxLength = 2000; urlInput.placeholder = "https://… veya /makaleler"; urlLabel.htmlFor = urlInput.id;
        const dialogError = make("p", "admin-link-dialog-error"); dialogError.setAttribute("role", "alert");
        const dialogActions = make("div", "admin-save-bar"); const cancel = make("button", "btn btn-outline-primary", "Vazgeç"); cancel.type = "button"; cancel.addEventListener("click", () => dialog.close());
        const add = make("button", "btn btn-primary", "Bağlantıyı ekle"); add.type = "submit";
        dialogActions.append(cancel, add); dialogForm.append(title, info, urlLabel, urlInput, dialogError, dialogActions); dialog.append(dialogForm); document.body.append(dialog);
        divider();
        button("Bağlantı", () => { urlInput.value = editor.getFormat().link || ""; dialogError.textContent = ""; dialog.showModal(); urlInput.focus(); });
        dialogForm.addEventListener("submit", event => {
            event.preventDefault(); const url = urlInput.value.trim();
            if (!allowedLink(url)) { announce(dialogError, "HTTPS/HTTP adresi, / ile başlayan site içi yol veya # bölüm bağlantısı girin."); urlInput.focus(); return; }
            dialog.close(); restoreSelection();
            if (selection.length) editor.formatText(selection.index, selection.length, "link", url, "user");
            else { editor.insertText(selection.index, url, "link", url, "user"); editor.setSelection(selection.index + url.length, 0, "silent"); }
        });
        keepDialogFocus(dialog);
        dialog.addEventListener("close", () => editor.focus());
        button("Bağlantıyı kaldır", () => editor.format("link", false, "user"));
        button("Geri al", () => editor.getModule("history").undo()); button("Yinele", () => editor.getModule("history").redo());
        button("Biçimi temizle", () => editor.removeFormat(selection.index, selection.length, "user"));
        const expand = button("Genişlet", () => { const expanded = wrapper.classList.toggle("is-expanded"); expand.setAttribute("aria-pressed", String(expanded)); expand.title = expanded ? "Daralt" : "Genişlet"; expand.setAttribute("aria-label", expand.title); }); expand.setAttribute("aria-pressed", "false");
        const previewDialog = make("dialog", "admin-editor-preview-dialog");
        previewDialog.setAttribute("aria-labelledby", `preview-title-${number}`);
        const previewTitle = make("h2"); previewTitle.id = `preview-title-${number}`;
        const previewBody = make("div", "admin-editor-preview-body");
        const closePreview = make("button", "btn btn-primary", "Kapat"); closePreview.type = "button"; closePreview.addEventListener("click", () => previewDialog.close());
        previewDialog.append(previewTitle, previewBody, closePreview); document.body.append(previewDialog);
        keepDialogFocus(previewDialog);
        previewDialog.addEventListener("close", () => editor.focus());
        const showPreview = (mode = "preview") => {
            sync(false); previewBody.replaceChildren();
            if (mode === "source") {
                previewTitle.textContent = "Kaynak görünümü";
                const code = make("textarea", "form-control admin-editor-source"); code.readOnly = true; code.value = source.value; code.setAttribute("aria-label", "Güvenli HTML kaynağı"); previewBody.append(code);
            } else if (mode === "stats") {
                previewTitle.textContent = "Kelime ve karakter sayısı"; previewBody.append(make("p", "", count.textContent));
            } else {
                previewTitle.textContent = "İçerik önizlemesi";
                const fragment = new DOMParser().parseFromString(source.value, "text/html");
                const content = make("div", "rich-content"); content.append(...fragment.body.childNodes); previewBody.append(content);
            }
            previewDialog.showModal(); closePreview.focus();
        };
        button("Önizleme", () => showPreview()); button("Kaynak görünümü", () => showPreview("source"));
        const invoke = name => () => commands.get(name)?.();
        menu("Dosya", [["Değişiklikleri kaydet", () => source.form?.requestSubmit()]]);
        menu("Düzen", [["Geri al", invoke("Geri al")], ["Yinele", invoke("Yinele")], ["Tümünü seç", () => { editor.focus(); editor.setSelection(0, editor.getLength() - 1); }]]);
        menu("Ekle", [["Bağlantı", invoke("Bağlantı")], ["Alıntı", invoke("Alıntı")], ["Madde listesi", invoke("Madde listesi")], ["Numaralı liste", invoke("Numaralı liste")]]);
        menu("Görünüm", [["Önizleme", () => showPreview()], ["Kaynak görünümü", () => showPreview("source")], ["Genişlet / daralt", invoke("Genişlet")]]);
        menu("Biçim", [["Kalın", invoke("Kalın")], ["İtalik", invoke("İtalik")], ["Altı çizili", invoke("Altı çizili")], ["Biçimi temizle", invoke("Biçimi temizle")]]);
        menu("Araçlar", [["Kelime ve karakter sayısı", () => showPreview("stats")], ["Bağlantıyı kaldır", invoke("Bağlantıyı kaldır")]]);
        function updateButtons() {
            // Reading without a range makes Quill focus the editor, stealing focus on blur/Tab.
            const format = editor.getFormat(selection.index, selection.length);
            controls.forEach(({control, format: name, value}) => control.setAttribute("aria-pressed", String(format[name] === value || (name === "align" && value === false && !format.align))));
            formatSelect.value = String(format.header || "");
        }
        const sync = (notify = true) => {
            // Export semantic lists, never Quill UI spans/classes/style attributes.
            const documentCopy = new DOMParser().parseFromString(editor.getSemanticHTML(), "text/html");
            documentCopy.body.querySelectorAll("*").forEach(element => {
                const alignment = element.getAttribute("class")?.match(/^ql-align-(center|right|justify)$/)?.[1];
                [...element.attributes].forEach(attribute => { if (!(element.tagName === "A" && attribute.name === "href")) element.removeAttribute(attribute.name); });
                if (alignment && ["P", "H2", "H3", "LI", "BLOCKQUOTE"].includes(element.tagName)) element.className = "content-align-" + alignment;
                if (element.tagName === "A" && !allowedLink(element.getAttribute("href"))) element.removeAttribute("href");
            });
            const text = editor.getText().trim(); source.value = text ? documentCopy.body.innerHTML : "";
            count.textContent = `${text.length.toLocaleString("tr-TR")} karakter · ${text ? text.split(/\s+/u).length : 0} kelime · kayıt: ${source.value.length.toLocaleString("tr-TR")}/${limit.toLocaleString("tr-TR")}`;
            if (source.value.length > limit) { error.hidden = false; error.textContent = `Biçimlendirme dahil içerik en fazla ${limit.toLocaleString("tr-TR")} karakter olabilir.`; editor.root.setAttribute("aria-invalid", "true"); }
            else { error.hidden = true; error.textContent = ""; editor.root.removeAttribute("aria-invalid"); }
            if (notify) source.dispatchEvent(new Event("input", {bubbles: true}));
        };
        editor.on("text-change", () => { sync(); updateButtons(); }); sync(false); updateButtons();
        editor.on("selection-change", range => { if (range) selection = range; updateButtons(); });
        const record = {source, editor, required, limit, sync, error}; editors.push(record);
        source.form?.addEventListener("submit", event => {
            sync(false);
            if ((required && !editor.getText().trim()) || source.value.length > limit) {
                event.preventDefault(); error.hidden = false; error.textContent = source.value.length > limit ? "İçerik karakter sınırını aşıyor; metni kısaltın." : "Bu içerik alanını doldurun.";
                editor.root.setAttribute("aria-invalid", "true"); editor.focus();
            }
        });
    });

    const dirtyForms = new Set();
    document.querySelectorAll("form[data-admin-form]").forEach(form => {
        const initial = JSON.stringify([...new FormData(form).entries()].filter(([name]) => name !== "__RequestVerificationToken"));
        const state = form.querySelector("[data-save-state]");
        const change = () => { const dirty = JSON.stringify([...new FormData(form).entries()].filter(([name]) => name !== "__RequestVerificationToken")) !== initial; dirty ? dirtyForms.add(form) : dirtyForms.delete(form); if (state) state.textContent = dirty ? "Kaydedilmemiş değişiklikler var" : "Henüz değişiklik yapılmadı"; };
        form.addEventListener("input", event => { const review = form.querySelector("#IsReviewed"); if (review && event.target !== review && event.target.id !== "IsActive") review.checked = false; change(); }); form.addEventListener("change", change);
        form.addEventListener("submit", event => {
            if (event.defaultPrevented) return;
            dirtyForms.delete(form); if (state) state.textContent = "Kaydediliyor…";
            form.querySelectorAll('button[type="submit"]').forEach(control => { control.disabled = true; control.textContent = form.enctype === "multipart/form-data" ? "Yükleniyor…" : "Kaydediliyor…"; });
        });
    });
    window.addEventListener("beforeunload", event => { if (dirtyForms.size) { event.preventDefault(); event.returnValue = ""; } });

    document.querySelectorAll(".admin-table").forEach((region, number) => {
        region.tabIndex = 0; region.setAttribute("role", "region"); region.setAttribute("aria-label", region.querySelector("caption")?.textContent || "Kayıt listesi");
        const rows = [...region.querySelectorAll("tbody > tr")]; if (rows.length < 2) return;
        const toolbar = make("div", "admin-toolbar"); const group = make("div"); const label = make("label", "", "Bu sayfada ara");
        const input = make("input", "form-control"); input.type = "search"; input.id = `admin-search-${number}`; label.htmlFor = input.id; input.placeholder = "Başlık veya kayıt bilgisi…";
        const hint = make("p", "form-text", "Arama yalnızca bu sayfada gösterilen kayıtları filtreler.");
        const result = make("p", "form-text"); result.setAttribute("role", "status"); group.append(label, input); toolbar.append(group, hint, result); region.before(toolbar);
        input.addEventListener("input", () => { const term = input.value.trim().toLocaleLowerCase("tr-TR"); rows.forEach(row => { row.hidden = !row.textContent.toLocaleLowerCase("tr-TR").includes(term); }); result.textContent = `${rows.filter(row => !row.hidden).length} / ${rows.length} kayıt gösteriliyor`; });
    });
    document.querySelectorAll("form.admin-delete").forEach(form => {
        const details = make("details", "admin-danger-zone"); const summary = make("summary", "", "Silme seçenekleri"); form.before(details); details.append(summary, form);
    });
    document.querySelectorAll("[data-seo-editor]").forEach(panel => {
        const title = panel.querySelector('input[data-seo="title"]'); const description = panel.querySelector('textarea[data-seo="description"]');
        const update = () => {
            panel.querySelector("[data-seo-preview-title]").textContent = (title?.value || "").trim() || (document.querySelector("input#Title, input#Name")?.value || "").trim() || "Sayfa başlığı";
            panel.querySelector("[data-seo-preview-description]").textContent = (description?.value || "").trim() || (document.querySelector("textarea#Excerpt, textarea#ShortDescription")?.value || "").trim() || "Sayfanız için kısa ve açıklayıcı bir meta açıklaması ekleyin.";
        };
        [title, description, document.querySelector("#Title, #Name"), document.querySelector("#Excerpt, #ShortDescription")].filter(Boolean).forEach(field => field.addEventListener("input", update)); update();
    });
    document.querySelectorAll("input[maxlength], textarea[maxlength]").forEach(field => {
        if (field.hasAttribute("data-rich-editor")) return;
        const counter = make("p", "admin-counter"); counter.id = field.id + "-counter";
        field.after(counter); field.setAttribute("aria-describedby", [field.getAttribute("aria-describedby"), counter.id].filter(Boolean).join(" "));
        const update = () => { counter.textContent = `${field.value.length} / ${field.maxLength} karakter`; }; field.addEventListener("input", update); update();
    });
    document.querySelectorAll('input[type="file"]').forEach(field => {
        const feedback = make("p", "form-text"); feedback.setAttribute("role", "status"); field.after(feedback);
        field.addEventListener("change", () => {
            const file = field.files[0]; if (!file) { feedback.textContent = ""; field.setCustomValidity(""); return; }
            const extensions = field.accept.includes("pdf") ? /\.pdf$/i : /\.(jpe?g|png|webp)$/i;
            const message = file.size > 5 * 1024 * 1024 ? "Dosya en fazla 5 MiB olabilir." : !extensions.test(file.name) ? "Desteklenen dosya türlerinden birini seçin." : "";
            field.setCustomValidity(message); feedback.textContent = message || `${file.name} · ${(file.size / 1024).toFixed(0)} KiB · yüklemeye hazır`;
            if (message) field.reportValidity();
        });
    });
    const published = document.querySelector("#PublishedAt");
    if (published) {
        const local = make("p", "form-text"); published.after(local);
        const update = () => { const date = new Date(published.value + "Z"); local.textContent = published.value && !Number.isNaN(date.getTime()) ? "Türkiye saati: " + new Intl.DateTimeFormat("tr-TR", {timeZone: "Europe/Istanbul", dateStyle: "medium", timeStyle: "short"}).format(date) : "Yayın zamanı seçildiğinde Türkiye saati burada gösterilir."; }; published.addEventListener("input", update); update();
    }
    const summary = document.querySelector(".validation-summary-errors, [data-validation-summary=true]");
    if (summary && summary.querySelector("li")) { summary.tabIndex = -1; summary.focus(); }
    document.querySelectorAll(".input-validation-error").forEach(field => { field.setAttribute("aria-invalid", "true"); for (let parent = field.parentElement; parent; parent = parent.parentElement) if (parent.tagName === "DETAILS") parent.open = true; });
})();
