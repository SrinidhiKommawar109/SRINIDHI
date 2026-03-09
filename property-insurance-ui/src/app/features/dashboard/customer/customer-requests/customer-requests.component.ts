import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
    PolicyRequestsService,
    PolicyRequest,
    SubmitPropertyPayload,
} from '../../../../core/policy-requests.service';
import { NotificationsService } from '../../../../core/notifications.service';

@Component({
    selector: 'app-customer-requests',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './customer-requests.component.html',
})
export class CustomerRequestsComponent implements OnInit {
    private readonly policies = inject(PolicyRequestsService);
    private readonly notifications = inject(NotificationsService);
    private readonly cdr = inject(ChangeDetectorRef);

    myRequests: PolicyRequest[] = [];
    requestsLoading = false;

    submitRequestId = 0;
    submitPayload: SubmitPropertyPayload = { propertyAddress: '', propertyValue: 0, propertyAge: 0 };
    submitMessage = '';

    buyRequestId = 0;
    buyMessage = '';
    showPaymentForm = false;

    ngOnInit(): void {
        this.loadMyRequests();
    }

    loadMyRequests(): void {
        this.requestsLoading = true;
        this.policies.getMyRequests().subscribe({
            next: (requests) => {
                this.myRequests = requests;
                this.requestsLoading = false;
                this.cdr.detectChanges();
            },
            error: () => {
                this.requestsLoading = false;
                this.cdr.detectChanges();
            },
        });
    }

    submitProperty(): void {
        if (!this.submitRequestId) {
            this.submitMessage = 'Please enter a valid Request ID.';
            return;
        }
        if (!this.submitPayload.propertyAddress.trim()) {
            this.submitMessage = 'Property address is required.';
            return;
        }
        if (this.submitPayload.propertyValue <= 0) {
            this.submitMessage = 'Property value must be greater than zero.';
            return;
        }
        if (this.submitPayload.propertyAge < 0) {
            this.submitMessage = 'Property age cannot be negative.';
            return;
        }

        this.submitMessage = '';
        this.policies.submitProperty(this.submitRequestId, this.submitPayload).subscribe({
            next: (msg) => {
                this.submitMessage = msg;
                this.notifications.show({ title: 'Details submitted', message: 'Property details sent to the assigned agent.', type: 'success' });
                this.loadMyRequests();
            },
            error: (err) => {
                this.submitMessage = err?.error || 'Something went wrong.';
                this.cdr.detectChanges();
            },
        });
    }

    // Payment form local state
    paymentInfo = { cardNumber: '', expiry: '', cvv: '' };
    paymentError = '';

    initiateCheckout(): void {
        if (!this.buyRequestId) return;
        this.paymentError = '';
        this.showPaymentForm = true;
    }

    canProceedToPayment(): boolean {
        if (!this.buyRequestId) return false;
        const req = this.myRequests.find(r => r.id === this.buyRequestId);
        return req?.status === 'RiskCalculated';
    }

    confirmPurchase(): void {
        if (!this.buyRequestId) return;

        // Basic Payment Validation
        if (this.paymentInfo.cardNumber.replace(/\D/g, '').length < 16) {
            this.paymentError = 'Invalid card number.';
            return;
        }
        if (!/^\d{2}\/\d{2}$/.test(this.paymentInfo.expiry)) {
            this.paymentError = 'Invalid expiry (MM/YY).';
            return;
        }
        if (this.paymentInfo.cvv.length < 3) {
            this.paymentError = 'Invalid CVV.';
            return;
        }

        this.buyMessage = '';
        this.paymentError = '';
        this.policies.buyPolicy(this.buyRequestId).subscribe({
            next: (msg) => {
                this.buyMessage = msg;
                this.notifications.show({ title: 'Purchase confirmed', message: 'Waiting for admin approval.', type: 'info' });
                this.showPaymentForm = false;
                this.loadMyRequests();
            },
            error: (err) => {
                this.buyMessage = err?.error || 'Something went wrong.';
                this.cdr.detectChanges();
            },
        });
    }
}
