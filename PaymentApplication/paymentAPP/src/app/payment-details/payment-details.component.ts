import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { PaymentDetail } from '../shared/payment-detail.model';
import { PaymentDetailService } from '../shared/payment-detail.service';

@Component({
  selector: 'app-payment-details',
  templateUrl: './payment-details.component.html',
  styleUrls: ['./payment-details.component.css']
})
export class PaymentDetailsComponent implements OnInit {

  constructor(public service: PaymentDetailService, private toastr: ToastrService) {

  }

  ngOnInit(): void {
    this.service.rfreshList();
  }

  populateForm(selectedRecord: PaymentDetail) {
    this.service.formData =Object.assign(selectedRecord);
    }

    OnDelete(id: number) {
      if(confirm('Are you sure that you want to delete this record?'))
      {
        this.service.deletePaymentDetail(id).subscribe(
          (response: any) => {
            this.service.rfreshList();
            this.toastr.error("Deleted successuffully", "Payment Detail Register")
          },
          err => {console.log(err)}
        )
      }
    }
}
