// Quantity stepper: +/- buttons adjust the adjacent number input.
document.addEventListener('click', function (e) {
    var btn = e.target.closest('[data-step]');
    if (!btn) return;
    var stepper = btn.closest('.ms-qty-stepper');
    if (!stepper) return;
    var input = stepper.querySelector('input[type=number]');
    if (!input) return;
    var min = parseInt(input.min || '1', 10);
    var max = input.max ? parseInt(input.max, 10) : Infinity;
    var value = parseInt(input.value || '1', 10) + parseInt(btn.getAttribute('data-step'), 10);
    input.value = Math.min(Math.max(value, min), max);
});

// Highlight the selected payment method card.
document.addEventListener('change', function (e) {
    if (e.target.name !== 'PaymentMethod') return;
    document.querySelectorAll('.ms-payment-option').forEach(function (el) {
        el.classList.toggle('selected', el.querySelector('input').checked);
    });
    var proofSection = document.getElementById('proof-upload');
    if (proofSection) {
        proofSection.style.display = e.target.value === 'Online' ? '' : 'none';
    }
});

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.ms-payment-option input:checked').forEach(function (input) {
        input.closest('.ms-payment-option').classList.add('selected');
    });
    var checkedMethod = document.querySelector('input[name=PaymentMethod]:checked');
    var proofSection = document.getElementById('proof-upload');
    if (checkedMethod && proofSection) {
        proofSection.style.display = checkedMethod.value === 'Online' ? '' : 'none';
    }
});
