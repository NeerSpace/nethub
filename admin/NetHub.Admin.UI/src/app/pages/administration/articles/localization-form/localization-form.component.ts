import { Component, Input, OnInit } from '@angular/core';
import { ArticleLocalizationModel, ContentStatus } from 'src/app/api';
import { IModalHandlers } from 'src/app/components/core/types';
import { ModalsService } from 'src/app/services/viewport';
import { LocalizationService } from '../localization.service';

@Component({
  selector: 'app-localization-form',
  templateUrl: './localization-form.component.html',
  styleUrls: ['./localization-form.component.scss'],
  providers: [LocalizationService],
})
export class LocalizationFormComponent implements OnInit {
  @Input() model!: ArticleLocalizationModel;

  locked: boolean = true;
  Status = ContentStatus;

  constructor(
    public readonly localizationsService: LocalizationService,
    private readonly modals: ModalsService,
  ) {}

  ngOnInit(): void {
    this.localizationsService.loadMetadata();
    this.localizationsService.form.setValue(this.model);
  }

  submit() {
    this.localizationsService.update(this.model.id);
    this.model.status = this.localizationsService.form.get('status')?.value;
  }

  onHotButtonPress(newStatus: ContentStatus) {
    const modelName = 'Article Localization';
    const handlers: IModalHandlers = {
      confirmed: () => {
        this.localizationsService.form.get('status')?.setValue(newStatus);
        this.submit();
      },
    };

    if (newStatus === ContentStatus.Banned) {
      this.modals.showConfirmBan(modelName, handlers);
    } else if (newStatus === ContentStatus.Draft) {
      this.modals.showConfirmLiftBan(modelName, handlers);
    } else if (newStatus === ContentStatus.Published) {
      this.modals.showConfirmPublish(modelName, handlers);
    } else {
      throw new Error(`Invalid new status passed: '${newStatus}'`);
    }
  }
}
