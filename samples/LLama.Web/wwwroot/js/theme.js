(() => {
    'use strict'

    const getStoredTheme = () => localStorage.getItem('theme')
    const setStoredTheme = theme => localStorage.setItem('theme', theme)

    const getAutoTheme = () => {
        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }

    const getPreferredTheme = () => {
        var storedTheme = getStoredTheme();

        if (!storedTheme || storedTheme == 'auto') {
            return getAutoTheme();
        }

        return storedTheme;
    }

    const setTheme = theme => {
        document.documentElement.setAttribute('data-bs-theme', getPreferredTheme());
    }

    setTheme(getPreferredTheme())

    const showActiveTheme = (theme) => {
        const themeSwitcher = document.querySelector('#bd-theme')

        if (!themeSwitcher) {
            return
        }

        const btnToSetActive = document.querySelector(`[data-bs-theme-value="${theme}"]`)

        themeSwitcher.innerHTML = btnToSetActive.innerHTML;

        document.querySelectorAll('[data-bs-theme-value]').forEach(element => {
            element.classList.remove('active')
            element.setAttribute('aria-pressed', 'false')
        })

        btnToSetActive.classList.add('active')
        btnToSetActive.setAttribute('aria-pressed', 'true')
    }

    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', () => {
        const storedTheme = getStoredTheme()
        if (storedTheme !== 'light' && storedTheme !== 'dark') {
            setTheme(getPreferredTheme())
        }
    })

    window.addEventListener('DOMContentLoaded', () => {
        showActiveTheme(getPreferredTheme())

        document.querySelectorAll('[data-bs-theme-value]')
            .forEach(toggle => {
                toggle.addEventListener('click', () => {
                    const theme = toggle.getAttribute('data-bs-theme-value')
                    setStoredTheme(theme)
                    setTheme(theme)
                    showActiveTheme(theme)
                })
            })
    })
})()