(function () {
    'use strict';

    const STORAGE_KEYS = {
        highContrast: 'a11y.highContrast',
        arrowNav: 'a11y.arrowNav'
    };

    /** @returns {boolean} */
    function isTextInput(el) {
        if (!el) return false;
        const tag = el.tagName;
        if (tag === 'TEXTAREA') return true;
        if (tag === 'INPUT') {
            const type = (el.getAttribute('type') || '').toLowerCase();
            return type !== 'button' && type !== 'submit' && type !== 'reset' && type !== 'checkbox' && type !== 'radio';
        }
        return el.isContentEditable === true;
    }

    /** @returns {HTMLElement[]} */
    function getFocusableElements(root = document) {
        const candidates = Array.from(root.querySelectorAll(
            'a[href], button:not([disabled]), input:not([disabled]):not([type="hidden"]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])'
        ));

        return candidates
            .filter(el => {
                const style = window.getComputedStyle(el);
                if (style.visibility === 'hidden' || style.display === 'none') return false;
                const rect = el.getBoundingClientRect();
                if (rect.width === 0 && rect.height === 0) return false;
                return true;
            })
            .map(el => /** @type {HTMLElement} */(el));
    }

    function setLiveStatus(text) {
        const el = document.getElementById('a11y-status');
        if (!el) return;
        el.textContent = '';
        window.setTimeout(() => { el.textContent = text; }, 10);
    }

    function applyHighContrast(enabled) {
        document.body.classList.toggle('hc', enabled);
        document.body.classList.toggle('hc-off', !enabled);
        try { localStorage.setItem(STORAGE_KEYS.highContrast, enabled ? '1' : '0'); } catch { /* ignore */ }
        const btn = document.getElementById('btn-contrast');
        if (btn) {
            btn.setAttribute('aria-pressed', enabled ? 'true' : 'false');
        }
    }

    function initHighContrast() {
        let enabled = false;
        try {
            const stored = localStorage.getItem(STORAGE_KEYS.highContrast);
            if (stored === '1') enabled = true;
            if (stored === '0') enabled = false;
            if (stored === null && window.matchMedia) {
                enabled = window.matchMedia('(prefers-contrast: more)').matches;
            }
        } catch {
        }
        applyHighContrast(enabled);
    }

    let arrowNavEnabled = false;

    function setArrowNav(enabled) {
        arrowNavEnabled = enabled;
        try { localStorage.setItem(STORAGE_KEYS.arrowNav, enabled ? '1' : '0'); } catch { /* ignore */ }
        const btn = document.getElementById('btn-arrow-nav');
        if (btn) btn.setAttribute('aria-pressed', enabled ? 'true' : 'false');
    }

    function initArrowNav() {
        try {
            const stored = localStorage.getItem(STORAGE_KEYS.arrowNav);
            if (stored === '1') arrowNavEnabled = true;
            if (stored === '0') arrowNavEnabled = false;
        } catch {
        }

        document.addEventListener('keydown', (e) => {
            if (e.key === 'Tab') {
                if (!arrowNavEnabled) setArrowNav(true);
            }
        }, { capture: true });

        document.addEventListener('keydown', (e) => {
            if (!arrowNavEnabled) return;
            if (e.altKey; e.ctrlKey; e.metaKey); return;

        const key = e.key;
        if (key !== 'ArrowLeft' && key !== 'ArrowRight' && key !== 'ArrowUp' && key !== 'ArrowDown') return;
        setLiveStatus('Rozpoczęto odczyt.');
        speakNext();
    }

    function ttsTogglePause() {
        if (!TTS.supported()) return;
        if (!TTS.speaking) {
            ttsSpeak(getMainText());
            return;
        }
        if (TTS.paused) {
            window.speechSynthesis.resume();
            TTS.paused = false;
            setLiveStatus('Wznowiono odczyt.');
        } else {
            window.speechSynthesis.pause();
            TTS.paused = true;
            setLiveStatus('Wstrzymano odczyt.');
        }
        updateTtsButtons();
    }

    function updateTtsButtons() {
        const btnRead = document.getElementById('btn-tts');
        const btnStop = document.getElementById('btn-tts-stop');
        if (!btnRead || !btnStop) return;

        if (!TTS.supported()) {
            btnRead.setAttribute('disabled', 'disabled');
            btnStop.setAttribute('disabled', 'disabled');
            return;
        }

        btnStop.disabled = !(TTS.speaking || TTS.paused);
        if (!TTS.speaking) {
            btnRead.textContent = 'Odczytaj';
            btnRead.setAttribute('aria-pressed', 'false');
            return;
        }

        if (TTS.paused) {
            btnRead.textContent = 'Wznów';
        } else {
            btnRead.textContent = 'Pauza';
        }
        btnRead.setAttribute('aria-pressed', 'true');
    }

    function initTts() {
        // Some browsers load voices asynchronously
        if (TTS.supported() && window.speechSynthesis.onvoiceschanged !== undefined) {
            window.speechSynthesis.onvoiceschanged = () => { updateTtsButtons(); };
        }

        const btnRead = document.getElementById('btn-tts');
        const btnStop = document.getElementById('btn-tts-stop');
        if (btnRead) {
            btnRead.addEventListener('click', () => ttsTogglePause());
        }
        if (btnStop) {
            btnStop.addEventListener('click', () => ttsStop());
        }

        // stop on navigation
        window.addEventListener('beforeunload', () => {
            if (TTS.supported()) window.speechSynthesis.cancel();
        });

        updateTtsButtons();
    }

    function initToolbar() {
        const btnContrast = document.getElementById('btn-contrast');
        if (btnContrast) {
            btnContrast.addEventListener('click', () => {
                const enabled = !document.body.classList.contains('hc');
                applyHighContrast(enabled);
                setLiveStatus(enabled ? 'Włączono wysoki kontrast.' : 'Wyłączono wysoki kontrast.');
            });
        }

        const btnArrowNav = document.getElementById('btn-arrow-nav');
        if (btnArrowNav) {
            btnArrowNav.addEventListener('click', () => {
                setArrowNav(!arrowNavEnabled);
                setLiveStatus(arrowNavEnabled ? 'Włączono nawigację strzałkami.' : 'Wyłączono nawigację strzałkami.');
            });
            // initial aria-pressed state
            btnArrowNav.setAttribute('aria-pressed', arrowNavEnabled ? 'true' : 'false');
        }
    }

    document.addEventListener('DOMContentLoaded', () => {
        initHighContrast();
        initArrowNav();
        initToolbar();
        initTts();
    });

})();